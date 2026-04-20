using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Rename.Profiles
{
    /// <summary>
    /// Single registration point for all profiles. Adding a 4th naming convention later
    /// = add one class + one line in the default provider. Open–closed.
    /// </summary>
    public interface IRenameProfileProvider
    {
        IReadOnlyList<IRenameProfile> GetProfiles();
    }
}
