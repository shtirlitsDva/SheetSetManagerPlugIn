using SheetSetManager.SheetManager.Rename.Engine;

using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Rename.Profiles
{
    /// <summary>
    /// One naming convention. Implementations are stateless and pure: given the same inputs
    /// they always produce the same preview rows. State (sequence counters, etc.) is passed
    /// in via <paramref name="seqState"/> on <see cref="BuildPreview"/> so the engine
    /// remains the only owner of iteration order.
    /// </summary>
    public interface IRenameProfile
    {
        /// <summary>Stable identifier — used as a settings key and as the legacy command's
        /// preselected profile id (e.g. "OLD", "VF", "NS").</summary>
        string Id { get; }

        /// <summary>Display name shown in the profile picker.</summary>
        string DisplayName { get; }

        /// <summary>Short description shown under the picker.</summary>
        string Description { get; }

        /// <summary>Field definitions rendered into the input panel, in display order.</summary>
        IReadOnlyList<RenameInputField> InputFields { get; }

        /// <summary>
        /// Compute the preview row for one sheet.
        /// </summary>
        /// <param name="sheet">Snapshot of the sheet's current state.</param>
        /// <param name="inputs">User-entered values, keyed by <see cref="RenameInputField.Name"/>.</param>
        /// <param name="seqState">Mutable counter the profile is free to increment.
        /// The engine creates one counter per Apply run and passes the same instance to
        /// every <see cref="BuildPreview"/> call so sequence numbers are global across
        /// subsets, matching the legacy behavior.</param>
        RenamePreviewRow BuildPreview(
            RenameSheetData sheet,
            IReadOnlyDictionary<string, string> inputs,
            RenameSequenceState seqState);
    }

    /// <summary>Mutable global counter handed by the engine to each profile call.</summary>
    public sealed class RenameSequenceState
    {
        public int Current { get; private set; }
        public int Next() => ++Current;
    }
}
