using SheetSetManager.SheetManager.Rename.Engine;
using SheetSetManager.SheetManager.Rename.Profiles;
using SheetSetManager.SheetManager.Rename.Profiles.Vf;

using System.Collections.Generic;

using Xunit;

namespace SheetSetManager.Tests.Rename.Profiles.Vf
{
    /// <summary>
    /// Validates the registered transition matrix inside <see cref="VfRenameProfile"/>.
    /// Drives via the public profile API rather than reaching into the table directly,
    /// so the test pins the externally-observable behavior. Test methods are
    /// non-parameterized so the assembly does not need to expose <see cref="VfSheetState"/>
    /// as a method parameter (it is internal).
    /// </summary>
    public sealed class VfTransitionTableTests
    {
        private static readonly Dictionary<string, string> InputsAll = new()
        {
            ["Program"] = "PROG",
            ["Komm"]    = "KOMM",
            ["Energi"]  = "ENERGI",
        };

        private static RenamePreviewRow Run(RenameSheetData sheet, string target)
        {
            var inputs = new Dictionary<string, string>(InputsAll) { ["SeparatorStyle"] = target };
            return new VfRenameProfile().BuildPreview(sheet, inputs, new RenameSequenceState());
        }

        [Fact]
        public void Civil_to_AllUnderscores_is_registered()
        {
            var row = Run(SheetFactory.Civil(), VfRenameProfile.SepAllUnderscores);
            Assert.NotEqual(RenameStatus.Error, row.Status);
        }

        [Fact]
        public void Civil_to_Dashes_is_registered()
        {
            var row = Run(SheetFactory.Civil(), VfRenameProfile.SepDashesExceptLast2);
            Assert.NotEqual(RenameStatus.Error, row.Status);
        }

        [Fact]
        public void VfAllUnderscores_to_Dashes_is_registered()
        {
            var row = Run(SheetFactory.VfAllUnderscores(), VfRenameProfile.SepDashesExceptLast2);
            Assert.NotEqual(RenameStatus.Error, row.Status);
        }

        [Fact]
        public void VfDashes_to_AllUnderscores_is_registered()
        {
            var row = Run(SheetFactory.VfDashes(), VfRenameProfile.SepAllUnderscores);
            Assert.NotEqual(RenameStatus.Error, row.Status);
        }

        [Fact]
        public void Same_state_AllUnderscores_is_NoChange()
        {
            var row = Run(SheetFactory.VfAllUnderscores(), VfRenameProfile.SepAllUnderscores);
            Assert.Equal(RenameStatus.NoChange, row.Status);
            Assert.Null(row.NewNumber);
            Assert.Null(row.NewTitle);
            Assert.Empty(row.PropertyChanges);
        }

        [Fact]
        public void Same_state_Dashes_is_NoChange()
        {
            var row = Run(SheetFactory.VfDashes(), VfRenameProfile.SepDashesExceptLast2);
            Assert.Equal(RenameStatus.NoChange, row.Status);
            Assert.Null(row.NewNumber);
            Assert.Null(row.NewTitle);
            Assert.Empty(row.PropertyChanges);
        }
    }
}
