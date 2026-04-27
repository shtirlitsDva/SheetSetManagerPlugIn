using SheetSetManager.SheetManager.Rename.Engine;
using SheetSetManager.SheetManager.Rename.Profiles;
using SheetSetManager.SheetManager.Rename.Profiles.Vf;

using System.Collections.Generic;

using Xunit;

namespace SheetSetManager.Tests.Rename.Profiles.Vf
{
    public sealed class VfSeparatorSwapStrategyTests
    {
        private static readonly Dictionary<string, string> NoInputs = new();

        [Fact]
        public void AllUnderscores_to_Dashes_rewrites_only_the_first_three_separators()
        {
            var strategy = new VfSeparatorSwapStrategy(
                VfStateClassifier.VfAllUnderscoresNumberRgx,
                "{0}-{1}-{2}_{3}_{4}");

            var sheet = SheetFactory.VfAllUnderscores(
                program: "PROG", komm: "KOMM", energi: "ENERGI", nr: 7, seq: 3);

            var row = strategy.Build(sheet, NoInputs, new RenameSequenceState());

            Assert.Equal("PROG-KOMM-ENERGI_007_003", row.NewNumber);
            Assert.Equal(RenameStatus.Matched, row.Status);
            // Title and Emnelinje must not be touched on a swap.
            Assert.Null(row.NewTitle);
            Assert.Empty(row.PropertyChanges);
        }

        [Fact]
        public void Dashes_to_AllUnderscores_rewrites_only_the_first_three_separators()
        {
            var strategy = new VfSeparatorSwapStrategy(
                VfStateClassifier.VfDashesExceptLast2NumberRgx,
                "{0}_{1}_{2}_{3}_{4}");

            var sheet = SheetFactory.VfDashes(
                program: "PROG", komm: "KOMM", energi: "ENERGI", nr: 7, seq: 3);

            var row = strategy.Build(sheet, NoInputs, new RenameSequenceState());

            Assert.Equal("PROG_KOMM_ENERGI_007_003", row.NewNumber);
            Assert.Equal(RenameStatus.Matched, row.Status);
            Assert.Null(row.NewTitle);
            Assert.Empty(row.PropertyChanges);
        }

        [Fact]
        public void Swap_does_not_advance_the_engine_sequence_counter()
        {
            var strategy = new VfSeparatorSwapStrategy(
                VfStateClassifier.VfAllUnderscoresNumberRgx,
                "{0}-{1}-{2}_{3}_{4}");

            var sheet = SheetFactory.VfAllUnderscores(seq: 42);
            var seqState = new RenameSequenceState();

            strategy.Build(sheet, NoInputs, seqState);

            Assert.Equal(0, seqState.Current);
        }

        [Fact]
        public void Swap_preserves_the_existing_seq_segment_verbatim()
        {
            // The user's existing numbering must survive a separator change.
            var strategy = new VfSeparatorSwapStrategy(
                VfStateClassifier.VfAllUnderscoresNumberRgx,
                "{0}-{1}-{2}_{3}_{4}");

            var sheet = SheetFactory.VfAllUnderscores(nr: 12, seq: 99);

            var row = strategy.Build(sheet, NoInputs, new RenameSequenceState());

            Assert.EndsWith("_012_099", row.NewNumber);
        }

        [Fact]
        public void Swap_preserves_arbitrary_prefix_text()
        {
            // Prefixes can be any non-separator characters; swap must round-trip them.
            var strategy = new VfSeparatorSwapStrategy(
                VfStateClassifier.VfAllUnderscoresNumberRgx,
                "{0}-{1}-{2}_{3}_{4}");

            var sheet = SheetFactory.VfAllUnderscores(
                program: "X1", komm: "Y2", energi: "Z3", nr: 1, seq: 1);

            var row = strategy.Build(sheet, NoInputs, new RenameSequenceState());

            Assert.Equal("X1-Y2-Z3_001_001", row.NewNumber);
        }
    }
}
