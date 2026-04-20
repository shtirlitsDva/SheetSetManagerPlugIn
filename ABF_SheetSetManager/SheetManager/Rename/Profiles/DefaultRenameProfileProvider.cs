using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Rename.Profiles
{
    /// <summary>
    /// Hardcoded registry of the three production profiles. To add a 4th convention:
    /// implement <see cref="IRenameProfile"/> and add one line here.
    /// </summary>
    internal sealed class DefaultRenameProfileProvider : IRenameProfileProvider
    {
        private readonly IReadOnlyList<IRenameProfile> _profiles = new IRenameProfile[]
        {
            new OldRenameProfile(),
            new VfRenameProfile(),
            new NsRenameProfile(),
        };

        public IReadOnlyList<IRenameProfile> GetProfiles() => _profiles;
    }
}
