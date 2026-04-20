using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Rename.Engine
{
    /// <summary>
    /// Immutable snapshot of one sheet as the rename engine sees it.
    /// Decoupled from <see cref="Models.SheetModel"/> so the engine remains pure
    /// and unit-testable without an AutoCAD process.
    /// </summary>
    /// <param name="OriginalIndex">
    /// Stable 0-based index back into the original sheet collection.
    /// The applier uses this to map a preview row to the live <see cref="Models.SheetModel"/>.
    /// </param>
    /// <param name="SubsetName">Name of the subset that contains this sheet — used by
    /// profiles whose token source is the subset name (e.g. RSSOLD).</param>
    /// <param name="Number">Current sheet number (intrinsic AcSmSheet property).</param>
    /// <param name="Title">Current sheet title (intrinsic AcSmSheet property).</param>
    /// <param name="CustomProperties">Snapshot of every custom property on this sheet.
    /// Profiles that emit Emnelinje 1/2 etc. read previous values here for the diff display.</param>
    public sealed record RenameSheetData(
        int OriginalIndex,
        string SubsetName,
        string Number,
        string Title,
        IReadOnlyDictionary<string, string?> CustomProperties);
}
