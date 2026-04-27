using SheetSetManager.SheetManager.Rename.Engine;

using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Rename.Profiles.Vf
{
    /// <summary>
    /// One concrete way to take a sheet from a known <see cref="VfSheetState"/> to a chosen
    /// target separator style. Strategies own all the per-transition logic — what to read,
    /// whether to consume user inputs, whether to advance the sequence counter, what fields
    /// to write back. The profile is a thin dispatcher over these.
    /// </summary>
    internal interface IVfRenameStrategy
    {
        /// <summary>
        /// Build the preview row for one sheet. The classifier has already verified the
        /// sheet's state matches what this strategy was registered for, so the strategy
        /// may assume the relevant inputs (title shape, number shape, custom properties)
        /// are well-formed.
        /// </summary>
        RenamePreviewRow Build(
            RenameSheetData sheet,
            IReadOnlyDictionary<string, string> inputs,
            RenameSequenceState seqState);
    }
}
