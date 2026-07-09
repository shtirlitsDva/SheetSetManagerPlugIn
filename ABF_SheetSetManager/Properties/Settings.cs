using System.Configuration;

namespace SheetSetManager.Properties
{
    /// <summary>
    /// Partial extension of the auto-generated <see cref="Settings"/> class. Lives in its own
    /// file so VS regenerating <c>Settings.Designer.cs</c> from <c>Settings.settings</c> does
    /// not delete the user-scoped properties we add here.
    /// </summary>
    internal sealed partial class Settings
    {
        /// <summary>
        /// JSON-serialized <c>Dictionary&lt;profileId, Dictionary&lt;inputName, value&gt;&gt;</c>.
        /// Persists the last-used inputs for each rename profile across AutoCAD sessions.
        /// </summary>
        [UserScopedSetting]
        [DefaultSettingValue("{}")]
        public string RenameProfileInputsJson
        {
            get => (string)(this[nameof(RenameProfileInputsJson)] ?? "{}");
            set => this[nameof(RenameProfileInputsJson)] = value;
        }
    }
}
