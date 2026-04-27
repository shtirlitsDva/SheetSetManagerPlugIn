using SheetSetManager.SheetManager.Rename.Engine;
using SheetSetManager.SheetManager.Rename.Profiles;
using SheetSetManager.SheetManager.Rename.Profiles.Vf;

using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace SheetSetManager.Tests.Rename.Profiles.Vf
{
    public sealed class CivilNascentToVfStrategyTests
    {
        private static readonly Dictionary<string, string> Inputs = new()
        {
            ["Program"] = "PROG",
            ["Komm"]    = "KOMM",
            ["Energi"]  = "ENERGI",
        };

        [Fact]
        public void Renders_AllUnderscores_number_and_sets_LEDNINGSPLAN_title_and_Emnelinje()
        {
            var strategy = new CivilNascentToVfStrategy(
                new RenameTemplate("{Program}_{Komm}_{Energi}_{NR:D3}_{Seq:D3}"));

            var sheet = SheetFactory.Civil(nr: 5, fst: "0+000", sst: "1+500");

            var row = strategy.Build(sheet, Inputs, new RenameSequenceState());

            Assert.Equal("PROG_KOMM_ENERGI_005_001", row.NewNumber);
            Assert.Equal("LEDNINGSPLAN", row.NewTitle);

            var changes = row.PropertyChanges.ToDictionary(c => c.Name);
            Assert.Equal("STRÆKNING 005", changes["Emnelinje 1"].NewValue);
            Assert.Equal("ST 0000 - 1500", changes["Emnelinje 2"].NewValue);

            Assert.Equal(RenameStatus.Matched, row.Status);
        }

        [Fact]
        public void Renders_DashesExceptLast2_number_when_using_dash_template()
        {
            var strategy = new CivilNascentToVfStrategy(
                new RenameTemplate("{Program}-{Komm}-{Energi}_{NR:D3}_{Seq:D3}"));

            var sheet = SheetFactory.Civil(nr: 5);

            var row = strategy.Build(sheet, Inputs, new RenameSequenceState());

            Assert.Equal("PROG-KOMM-ENERGI_005_001", row.NewNumber);
            Assert.Equal(RenameStatus.Matched, row.Status);
        }

        [Fact]
        public void Sequence_advances_globally_across_calls()
        {
            var strategy = new CivilNascentToVfStrategy(
                new RenameTemplate("{Program}_{Komm}_{Energi}_{NR:D3}_{Seq:D3}"));

            var seq = new RenameSequenceState();

            var row1 = strategy.Build(SheetFactory.Civil(index: 0, nr: 11), Inputs, seq);
            var row2 = strategy.Build(SheetFactory.Civil(index: 1, nr: 22), Inputs, seq);

            Assert.EndsWith("_011_001", row1.NewNumber);
            Assert.EndsWith("_022_002", row2.NewNumber);
        }

        [Fact]
        public void Strips_plus_signs_from_FST_and_SST_when_writing_Emnelinje_2()
        {
            var strategy = new CivilNascentToVfStrategy(
                new RenameTemplate("{Program}_{Komm}_{Energi}_{NR:D3}_{Seq:D3}"));

            var sheet = SheetFactory.Civil(nr: 9, fst: "2+125", sst: "3+875");

            var row = strategy.Build(sheet, Inputs, new RenameSequenceState());

            var emne2 = row.PropertyChanges.Single(c => c.Name == "Emnelinje 2");
            Assert.Equal("ST 2125 - 3875", emne2.NewValue);
        }
    }
}
