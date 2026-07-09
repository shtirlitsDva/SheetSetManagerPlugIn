using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Rename.Profiles.Vf
{
    /// <summary>
    /// Registered transitions <c>(sourceState × targetSeparator) → strategy</c>.
    /// A miss means the combination is not a supported operation and the profile
    /// emits an error row for that sheet.
    /// </summary>
    internal sealed class VfTransitionTable
    {
        private readonly Dictionary<(VfSheetState, string), IVfRenameStrategy> _table;

        public VfTransitionTable(Dictionary<(VfSheetState, string), IVfRenameStrategy> table)
        {
            _table = table;
        }

        public IVfRenameStrategy? Resolve(VfSheetState state, string targetSeparator)
            => _table.TryGetValue((state, targetSeparator), out var s) ? s : null;
    }
}
