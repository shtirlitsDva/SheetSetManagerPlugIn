using ACSMCOMPONENTS25Lib;

using SheetSetManager.SheetManager.Rename.Engine;
using SheetSetManager.Wrappers;

using System;
using System.Collections.Generic;

using static SheetSetManager.Utils;

namespace SheetSetManager.SheetManager.Rename.Interop
{
    /// <summary>
    /// Captures one snapshot of the open SSM database for the rename engine, AND keeps the
    /// parallel <see cref="IAcSmObjectId"/> handles needed by the applier to write back.
    /// </summary>
    /// <remarks>
    /// Why not reuse <see cref="SheetSetManager.SheetManager.Managers.SheetsManager"/>? It flattens
    /// subset structure away (the OLD profile needs subset names) and snapshots only intrinsic
    /// number, not custom-property values. A separate, narrowly-scoped reader keeps both responsibilities
    /// clean.
    /// </remarks>
    internal sealed class RenameSheetSource
    {
        private readonly AcSmDatabase _db;
        private readonly IAcSmObjectId[] _sheetOids;

        private RenameSheetSource(
            AcSmDatabase db,
            IReadOnlyList<RenameSheetData> snapshots,
            IAcSmObjectId[] sheetOids)
        {
            _db = db;
            Snapshots = snapshots;
            _sheetOids = sheetOids;
        }

        public IReadOnlyList<RenameSheetData> Snapshots { get; }

        /// <summary>Resolve the live AcSmSheet for a preview row's <c>OriginalIndex</c>.</summary>
        public AcSmSheet ResolveSheet(int originalIndex)
        {
            var oid = _sheetOids[originalIndex];
            if (oid.GetPersistObject() is not AcSmSheet sheet)
                throw new InvalidOperationException(
                    $"Sheet at index {originalIndex} is no longer present in the database.");
            return sheet;
        }

        public void Lock() => _db.LockDb(_db);
        public void Unlock(bool commit) => _db.UnlockDb(_db, commit);

        /// <summary>Open the single .dst, walk subsets→sheets and snapshot each one.</summary>
        public static RenameSheetSource Load()
        {
            IAcSmSheetSetMgr ssm = new AcSmSheetSetMgr();
            var enumDb = ssm.GetDatabaseEnumerator();

            // Enforce single-database invariant (matches Interop.SheetSetManager).
            int dbCount = 0;
            IAcSmPersist? probe = enumDb.Next();
            while (probe != null) { dbCount++; probe = enumDb.Next(); }
            if (dbCount == 0) throw new InvalidOperationException(
                "No sheet set database is open. Open exactly one .dst and try again.");
            if (dbCount > 1) throw new InvalidOperationException(
                "More than one sheet set database is open. Close all but one .dst and try again.");

            enumDb.Reset();
            var first = enumDb.Next();
            var db = first.GetDatabase();
            var sheetSet = db.GetSheetSet();

            var snapshots = new List<RenameSheetData>();
            var oids = new List<IAcSmObjectId>();
            int idx = 0;

            foreach (var subsetComp in new AcSmComEnumerator(sheetSet.GetSheetEnumerator()))
            {
                if (subsetComp.GetTypeName() != "AcSmSubset") continue;
                var subset = (AcSmSubset)subsetComp;
                var subsetName = subset.GetName();

                foreach (var sheetComp in new AcSmComEnumerator(subset.GetSheetEnumerator()))
                {
                    if (sheetComp.GetTypeName() != "AcSmSheet") continue;
                    var sheet = (AcSmSheet)sheetComp;

                    var customProps = ReadCustomProperties(sheet);

                    snapshots.Add(new RenameSheetData(
                        OriginalIndex: idx,
                        SubsetName: subsetName,
                        Number: sheet.GetNumber() ?? string.Empty,
                        Title: sheet.GetTitle() ?? string.Empty,
                        CustomProperties: customProps));

                    oids.Add(sheet.GetObjectId());
                    idx++;
                }
            }

            prtDbg($"RenameSheetSource: snapshotted {snapshots.Count} sheets.");
            return new RenameSheetSource(db, snapshots, oids.ToArray());
        }

        private static IReadOnlyDictionary<string, string?> ReadCustomProperties(AcSmSheet sheet)
        {
            var dict = new Dictionary<string, string?>(StringComparer.Ordinal);
            var bag = sheet.GetCustomPropertyBag();
            foreach (var (name, value) in new AcSmPropertyEnumerator(bag.GetPropertyEnumerator()))
            {
                dict[name] = value.GetValue() as string;
            }
            return dict;
        }
    }
}
