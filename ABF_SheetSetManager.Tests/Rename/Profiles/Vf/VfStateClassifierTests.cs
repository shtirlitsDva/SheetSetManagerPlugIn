using SheetSetManager.SheetManager.Rename.Engine;
using SheetSetManager.SheetManager.Rename.Profiles.Vf;

using System.Collections.Generic;

using Xunit;

namespace SheetSetManager.Tests.Rename.Profiles.Vf
{
    public sealed class VfStateClassifierTests
    {
        private readonly VfStateClassifier _sut = new();

        [Fact]
        public void Civil_source_title_classifies_as_CivilNascent()
        {
            var sheet = SheetFactory.Civil();
            Assert.Equal(VfSheetState.CivilNascent, _sut.Classify(sheet));
        }

        [Theory]
        [InlineData("3 ST -2+250 - 4+750")]
        [InlineData("12 ST 0+000 - 1+500")]            // no leading dash on FST
        [InlineData("12 ST -0+000.123 - 1+500")]       // FST with trailing fractional
        [InlineData("12 ST -0+000 - 1+500.456")]       // SST with trailing digits
        public void Civil_title_variants_all_classify_as_CivilNascent(string title)
        {
            var sheet = new RenameSheetData(
                OriginalIndex: 0,
                SubsetName: "S",
                Number: "anything",
                Title: title,
                CustomProperties: new Dictionary<string, string?>());
            Assert.Equal(VfSheetState.CivilNascent, _sut.Classify(sheet));
        }

        [Fact]
        public void Vf_all_underscores_number_with_LEDNINGSPLAN_title_classifies_correctly()
        {
            var sheet = SheetFactory.VfAllUnderscores();
            Assert.Equal(VfSheetState.VfAllUnderscores, _sut.Classify(sheet));
        }

        [Fact]
        public void Vf_dashes_number_with_LEDNINGSPLAN_title_classifies_correctly()
        {
            var sheet = SheetFactory.VfDashes();
            Assert.Equal(VfSheetState.VfDashesExceptLast2, _sut.Classify(sheet));
        }

        [Fact]
        public void Vf_shape_with_wrong_title_is_Unknown()
        {
            // Number shape is correct, but Title is not the literal LEDNINGSPLAN
            // and is not a Civil-source title either.
            var sheet = new RenameSheetData(
                OriginalIndex: 0,
                SubsetName: "S",
                Number: "PROG_KOMM_ENERGI_005_001",
                Title: "Not LEDNINGSPLAN",
                CustomProperties: new Dictionary<string, string?>());
            Assert.Equal(VfSheetState.Unknown, _sut.Classify(sheet));
        }

        [Fact]
        public void LEDNINGSPLAN_title_with_unrecognized_number_is_Unknown()
        {
            var sheet = new RenameSheetData(
                OriginalIndex: 0,
                SubsetName: "S",
                Number: "free-form-string-not-matching-anything",
                Title: "LEDNINGSPLAN",
                CustomProperties: new Dictionary<string, string?>());
            Assert.Equal(VfSheetState.Unknown, _sut.Classify(sheet));
        }

        [Fact]
        public void Discrimination_between_VF_shapes_is_unambiguous()
        {
            // Same prefixes; only the separator differs.
            var underscoreSheet = SheetFactory.VfAllUnderscores(program: "AAA", komm: "BBB", energi: "CCC");
            var dashSheet       = SheetFactory.VfDashes(program: "AAA", komm: "BBB", energi: "CCC");

            Assert.Equal(VfSheetState.VfAllUnderscores,    _sut.Classify(underscoreSheet));
            Assert.Equal(VfSheetState.VfDashesExceptLast2, _sut.Classify(dashSheet));
        }

        [Fact]
        public void Civil_title_wins_even_if_number_happens_to_match_a_VF_shape()
        {
            // Edge case: a sheet that for whatever reason has a VF-shaped number
            // but the title still carries Civil tokens. Title wins — Civil-state
            // sheets are pre-rename and should be treated as such.
            var sheet = new RenameSheetData(
                OriginalIndex: 0,
                SubsetName: "S",
                Number: "PROG_KOMM_ENERGI_005_001",
                Title: "5 ST -0+000 - 1+500",
                CustomProperties: new Dictionary<string, string?>());
            Assert.Equal(VfSheetState.CivilNascent, _sut.Classify(sheet));
        }
    }
}
