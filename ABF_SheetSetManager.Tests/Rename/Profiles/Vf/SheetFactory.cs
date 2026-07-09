using SheetSetManager.SheetManager.Rename.Engine;

using System.Collections.Generic;

namespace SheetSetManager.Tests.Rename.Profiles.Vf
{
    /// <summary>
    /// Test-only helper for constructing <see cref="RenameSheetData"/> snapshots in the
    /// shapes the production code expects to encounter. Centralizes the magic strings so
    /// individual tests stay readable.
    /// </summary>
    internal static class SheetFactory
    {
        /// <summary>Sheet straight from the Civil sheet-creation tool.</summary>
        public static RenameSheetData Civil(
            int index = 0,
            string subset = "Subset 01",
            string number = "Sheet 1",
            int nr = 5,
            string fst = "0+000",
            string sst = "1+500")
        {
            var title = $"{nr} ST -{fst} - {sst}";
            return new RenameSheetData(
                OriginalIndex: index,
                SubsetName: subset,
                Number: number,
                Title: title,
                CustomProperties: Empty());
        }

        /// <summary>Sheet already in VF naming with all-underscore separators.</summary>
        public static RenameSheetData VfAllUnderscores(
            int index = 0,
            string subset = "Subset 01",
            string program = "PROG",
            string komm = "KOMM",
            string energi = "ENERGI",
            int nr = 5,
            int seq = 1,
            string fstNoPlus = "0000",
            string sstNoPlus = "1500")
        {
            var number = $"{program}_{komm}_{energi}_{nr:D3}_{seq:D3}";
            return new RenameSheetData(
                OriginalIndex: index,
                SubsetName: subset,
                Number: number,
                Title: "LEDNINGSPLAN",
                CustomProperties: WithEmnelinje(nr, fstNoPlus, sstNoPlus));
        }

        /// <summary>Sheet already in VF naming with dashes between the first three segments.</summary>
        public static RenameSheetData VfDashes(
            int index = 0,
            string subset = "Subset 01",
            string program = "PROG",
            string komm = "KOMM",
            string energi = "ENERGI",
            int nr = 5,
            int seq = 1,
            string fstNoPlus = "0000",
            string sstNoPlus = "1500")
        {
            var number = $"{program}-{komm}-{energi}_{nr:D3}_{seq:D3}";
            return new RenameSheetData(
                OriginalIndex: index,
                SubsetName: subset,
                Number: number,
                Title: "LEDNINGSPLAN",
                CustomProperties: WithEmnelinje(nr, fstNoPlus, sstNoPlus));
        }

        /// <summary>An arbitrary sheet that does not match any VF state.</summary>
        public static RenameSheetData Unknown(
            int index = 0,
            string subset = "Subset 01",
            string number = "GARBAGE",
            string title = "totally unrelated title")
        {
            return new RenameSheetData(
                OriginalIndex: index,
                SubsetName: subset,
                Number: number,
                Title: title,
                CustomProperties: Empty());
        }

        private static IReadOnlyDictionary<string, string?> Empty()
            => new Dictionary<string, string?>();

        private static IReadOnlyDictionary<string, string?> WithEmnelinje(int nr, string fst, string sst)
            => new Dictionary<string, string?>
            {
                ["Emnelinje 1"] = $"STRÆKNING {nr:D3}",
                ["Emnelinje 2"] = $"ST {fst} - {sst}",
            };
    }
}
