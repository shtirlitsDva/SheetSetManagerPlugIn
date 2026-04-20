using SheetSetManager.SheetManager.Rename.Profiles;

using System;
using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Rename.Engine
{
    /// <summary>
    /// Pure orchestrator: walks the input sheets in order, hands each to the profile,
    /// and collects preview rows. No SSM/AutoCAD dependency — fully unit-testable.
    /// </summary>
    public sealed class RenameEngine
    {
        public IReadOnlyList<RenamePreviewRow> BuildPreview(
            IRenameProfile profile,
            IReadOnlyDictionary<string, string> inputs,
            IReadOnlyList<RenameSheetData> sheets)
        {
            if (profile is null) throw new ArgumentNullException(nameof(profile));
            if (inputs is null) throw new ArgumentNullException(nameof(inputs));
            if (sheets is null) throw new ArgumentNullException(nameof(sheets));

            var seq = new RenameSequenceState();
            var rows = new List<RenamePreviewRow>(sheets.Count);

            foreach (var sheet in sheets)
            {
                RenamePreviewRow row;
                try
                {
                    row = profile.BuildPreview(sheet, inputs, seq);
                }
                catch (Exception ex)
                {
                    row = new RenamePreviewRow(
                        sheet.OriginalIndex,
                        sheet.SubsetName,
                        sheet.Number, null,
                        sheet.Title, null,
                        Array.Empty<RenamePropertyChange>(),
                        RenameStatus.Error,
                        $"Profile threw: {ex.Message}");
                }
                rows.Add(row);
            }

            return rows;
        }
    }
}
