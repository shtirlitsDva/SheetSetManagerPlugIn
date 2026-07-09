using SheetSetManager.SheetManager.Rename.Engine;

using System;
using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Rename.Profiles.Vf
{
    /// <summary>
    /// Source state already equals the requested target. Emit a no-change row so the
    /// preview shows it explicitly rather than treating idempotent runs as errors.
    /// </summary>
    internal sealed class VfNoChangeStrategy : IVfRenameStrategy
    {
        public RenamePreviewRow Build(
            RenameSheetData sheet,
            IReadOnlyDictionary<string, string> inputs,
            RenameSequenceState seqState)
        {
            return new RenamePreviewRow(
                sheet.OriginalIndex, sheet.SubsetName,
                sheet.Number, null,
                sheet.Title, null,
                Array.Empty<RenamePropertyChange>(),
                RenameStatus.NoChange);
        }
    }
}
