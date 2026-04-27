using SheetSetManager.SheetManager.Rename.Engine;
using SheetSetManager.SheetManager.Rename.Profiles;

using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace SheetSetManager.Tests.Rename.Profiles.Vf
{
    /// <summary>
    /// End-to-end via <see cref="RenameEngine"/>. This is the path the production
    /// VM walks, just without the AutoCAD-bound source/applier. Verifies that the
    /// classifier + transition table + strategies compose into the full preview.
    /// </summary>
    public sealed class VfEndToEndTests
    {
        private static IReadOnlyDictionary<string, string> Inputs(
            string separator,
            string program = "PROG",
            string komm = "KOMM",
            string energi = "ENERGI")
            => new Dictionary<string, string>
            {
                ["Program"] = program,
                ["Komm"]    = komm,
                ["Energi"]  = energi,
                ["SeparatorStyle"] = separator,
            };

        [Fact]
        public void Civil_to_VF_AllUnderscores_renames_full_set()
        {
            var sheets = new[]
            {
                SheetFactory.Civil(0, nr: 1),
                SheetFactory.Civil(1, nr: 2),
                SheetFactory.Civil(2, nr: 3),
            };

            var rows = new RenameEngine().BuildPreview(
                new VfRenameProfile(),
                Inputs(VfRenameProfile.SepAllUnderscores),
                sheets);

            Assert.Equal(3, rows.Count);
            Assert.Equal("PROG_KOMM_ENERGI_001_001", rows[0].NewNumber);
            Assert.Equal("PROG_KOMM_ENERGI_002_002", rows[1].NewNumber);
            Assert.Equal("PROG_KOMM_ENERGI_003_003", rows[2].NewNumber);
            Assert.All(rows, r => Assert.Equal(RenameStatus.Matched, r.Status));
        }

        [Fact]
        public void Civil_to_VF_Dashes_renames_full_set()
        {
            var sheets = new[]
            {
                SheetFactory.Civil(0, nr: 1),
                SheetFactory.Civil(1, nr: 2),
            };

            var rows = new RenameEngine().BuildPreview(
                new VfRenameProfile(),
                Inputs(VfRenameProfile.SepDashesExceptLast2),
                sheets);

            Assert.Equal("PROG-KOMM-ENERGI_001_001", rows[0].NewNumber);
            Assert.Equal("PROG-KOMM-ENERGI_002_002", rows[1].NewNumber);
        }

        [Fact]
        public void Vf_AllUnderscores_to_VF_Dashes_swaps_separators_no_inputs_needed()
        {
            // Already-renamed VF sheets, user picks the other separator.
            var sheets = new[]
            {
                SheetFactory.VfAllUnderscores(0, program: "A", komm: "B", energi: "C", nr: 1, seq: 1),
                SheetFactory.VfAllUnderscores(1, program: "A", komm: "B", energi: "C", nr: 2, seq: 2),
                SheetFactory.VfAllUnderscores(2, program: "A", komm: "B", energi: "C", nr: 3, seq: 3),
            };

            // No Program/Komm/Energi — transition shouldn't need them.
            var inputs = new Dictionary<string, string>
            {
                ["SeparatorStyle"] = VfRenameProfile.SepDashesExceptLast2,
            };

            var rows = new RenameEngine().BuildPreview(new VfRenameProfile(), inputs, sheets);

            Assert.Equal(3, rows.Count);
            Assert.Equal("A-B-C_001_001", rows[0].NewNumber);
            Assert.Equal("A-B-C_002_002", rows[1].NewNumber);
            Assert.Equal("A-B-C_003_003", rows[2].NewNumber);
            Assert.All(rows, r => Assert.Equal(RenameStatus.Matched, r.Status));
            // The swap leaves Title and Emnelinje alone.
            Assert.All(rows, r => Assert.Null(r.NewTitle));
            Assert.All(rows, r => Assert.Empty(r.PropertyChanges));
        }

        [Fact]
        public void Vf_Dashes_to_VF_AllUnderscores_swaps_separators_no_inputs_needed()
        {
            var sheets = new[]
            {
                SheetFactory.VfDashes(0, program: "A", komm: "B", energi: "C", nr: 1, seq: 1),
                SheetFactory.VfDashes(1, program: "A", komm: "B", energi: "C", nr: 2, seq: 2),
            };

            var inputs = new Dictionary<string, string>
            {
                ["SeparatorStyle"] = VfRenameProfile.SepAllUnderscores,
            };

            var rows = new RenameEngine().BuildPreview(new VfRenameProfile(), inputs, sheets);

            Assert.Equal("A_B_C_001_001", rows[0].NewNumber);
            Assert.Equal("A_B_C_002_002", rows[1].NewNumber);
        }

        [Fact]
        public void Same_state_picked_for_target_yields_NoChange_for_every_sheet()
        {
            var sheets = new[]
            {
                SheetFactory.VfAllUnderscores(0),
                SheetFactory.VfAllUnderscores(1),
            };

            var inputs = new Dictionary<string, string>
            {
                ["SeparatorStyle"] = VfRenameProfile.SepAllUnderscores,
            };

            var rows = new RenameEngine().BuildPreview(new VfRenameProfile(), inputs, sheets);

            Assert.All(rows, r => Assert.Equal(RenameStatus.NoChange, r.Status));
        }

        [Fact]
        public void Round_trip_preserves_original_number()
        {
            // Civil → AllUnderscores, then back: AllUnderscores → Dashes → AllUnderscores
            // should land on the exact same string. Pin this so we know the swap is lossless.
            var civil = SheetFactory.Civil(0, nr: 7);

            var step1 = new RenameEngine().BuildPreview(
                new VfRenameProfile(),
                Inputs(VfRenameProfile.SepAllUnderscores),
                new[] { civil });
            var afterAll = step1[0].NewNumber!;

            var allSheet = SheetFactory.VfAllUnderscores(
                0, program: "PROG", komm: "KOMM", energi: "ENERGI", nr: 7, seq: 1);

            var step2 = new RenameEngine().BuildPreview(
                new VfRenameProfile(),
                new Dictionary<string, string> { ["SeparatorStyle"] = VfRenameProfile.SepDashesExceptLast2 },
                new[] { allSheet });
            var afterDash = step2[0].NewNumber!;

            // Reconstruct the dashed sheet and swap back.
            var dashSheet = SheetFactory.VfDashes(
                0, program: "PROG", komm: "KOMM", energi: "ENERGI", nr: 7, seq: 1);

            var step3 = new RenameEngine().BuildPreview(
                new VfRenameProfile(),
                new Dictionary<string, string> { ["SeparatorStyle"] = VfRenameProfile.SepAllUnderscores },
                new[] { dashSheet });
            var afterAllAgain = step3[0].NewNumber!;

            Assert.Equal("PROG_KOMM_ENERGI_007_001", afterAll);
            Assert.Equal("PROG-KOMM-ENERGI_007_001", afterDash);
            Assert.Equal(afterAll, afterAllAgain);
        }

        [Fact]
        public void Unknown_state_produces_an_Error_row_with_explanatory_message()
        {
            var sheets = new[] { SheetFactory.Unknown(0) };

            var rows = new RenameEngine().BuildPreview(
                new VfRenameProfile(),
                Inputs(VfRenameProfile.SepAllUnderscores),
                sheets);

            Assert.Single(rows);
            Assert.Equal(RenameStatus.Error, rows[0].Status);
            Assert.Contains("known VF state", rows[0].ErrorMessage!);
        }
    }
}
