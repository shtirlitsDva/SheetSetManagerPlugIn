using SheetSetManager.SheetManager.Rename.Engine;
using SheetSetManager.SheetManager.Rename.Profiles;

using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace SheetSetManager.Tests.Rename.Profiles.Vf
{
    public sealed class VfRenameProfilePreflightTests
    {
        private readonly VfRenameProfile _sut = new();

        [Fact]
        public void Empty_set_returns_full_inputs_no_errors()
        {
            var pf = _sut.Preflight(new List<RenameSheetData>());

            Assert.Empty(pf.FatalErrors);
            Assert.Equal(_sut.InputFields.Count, pf.RequiredInputs.Count);
        }

        [Fact]
        public void All_Civil_returns_full_inputs_no_errors()
        {
            var sheets = new[]
            {
                SheetFactory.Civil(0),
                SheetFactory.Civil(1),
            };

            var pf = _sut.Preflight(sheets);

            Assert.Empty(pf.FatalErrors);
            // Full input set: Program, Komm, Energi, SeparatorStyle.
            Assert.Equal(4, pf.RequiredInputs.Count);
            Assert.Contains(pf.RequiredInputs, f => f.Name == "Program");
            Assert.Contains(pf.RequiredInputs, f => f.Name == "Komm");
            Assert.Contains(pf.RequiredInputs, f => f.Name == "Energi");
            Assert.Contains(pf.RequiredInputs, f => f.Name == "SeparatorStyle");
        }

        [Fact]
        public void All_VfAllUnderscores_returns_separator_only_no_errors()
        {
            var sheets = new[]
            {
                SheetFactory.VfAllUnderscores(0),
                SheetFactory.VfAllUnderscores(1),
            };

            var pf = _sut.Preflight(sheets);

            Assert.Empty(pf.FatalErrors);
            Assert.Single(pf.RequiredInputs);
            Assert.Equal("SeparatorStyle", pf.RequiredInputs[0].Name);
        }

        [Fact]
        public void All_VfDashes_returns_separator_only_no_errors()
        {
            var sheets = new[]
            {
                SheetFactory.VfDashes(0),
                SheetFactory.VfDashes(1),
            };

            var pf = _sut.Preflight(sheets);

            Assert.Empty(pf.FatalErrors);
            Assert.Single(pf.RequiredInputs);
            Assert.Equal("SeparatorStyle", pf.RequiredInputs[0].Name);
        }

        [Fact]
        public void Mixed_Civil_and_Vf_is_a_fatal_error()
        {
            var sheets = new[]
            {
                SheetFactory.Civil(0),
                SheetFactory.VfAllUnderscores(1),
            };

            var pf = _sut.Preflight(sheets);

            Assert.Empty(pf.RequiredInputs);
            Assert.Single(pf.FatalErrors);
            Assert.Contains("Mixed", pf.FatalErrors[0]);
        }

        [Fact]
        public void Mixed_two_VF_states_is_a_fatal_error()
        {
            var sheets = new[]
            {
                SheetFactory.VfAllUnderscores(0),
                SheetFactory.VfDashes(1),
            };

            var pf = _sut.Preflight(sheets);

            Assert.Empty(pf.RequiredInputs);
            Assert.Single(pf.FatalErrors);
            Assert.Contains("Mixed", pf.FatalErrors[0]);
        }

        [Fact]
        public void Any_unknown_sheet_is_a_fatal_error()
        {
            var sheets = new[]
            {
                SheetFactory.VfAllUnderscores(0),
                SheetFactory.Unknown(1),
            };

            var pf = _sut.Preflight(sheets);

            Assert.Empty(pf.RequiredInputs);
            Assert.Single(pf.FatalErrors);
            Assert.Contains("could not be classified", pf.FatalErrors[0]);
        }

        [Fact]
        public void Unknown_only_is_also_a_fatal_error()
        {
            var sheets = new[] { SheetFactory.Unknown(0) };

            var pf = _sut.Preflight(sheets);

            Assert.Empty(pf.RequiredInputs);
            Assert.Single(pf.FatalErrors);
        }
    }
}
