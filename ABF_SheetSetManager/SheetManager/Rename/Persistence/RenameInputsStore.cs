using SheetSetManager.Properties;

using System;
using System.Collections.Generic;
using System.Text.Json;

using static SheetSetManager.Utils;

namespace SheetSetManager.SheetManager.Rename.Persistence
{
    /// <summary>
    /// Reads and writes per-profile last-used input values. Backed by
    /// <see cref="Settings.RenameProfileInputsJson"/>, so values persist in the standard
    /// per-user .config file managed by <see cref="System.Configuration.ApplicationSettingsBase"/>.
    /// Failures (e.g. corrupt JSON) degrade gracefully: the user just gets empty defaults.
    /// </summary>
    internal sealed class RenameInputsStore
    {
        public IReadOnlyDictionary<string, string> Load(string profileId)
        {
            var all = LoadAll();
            return all.TryGetValue(profileId, out var inputs)
                ? inputs
                : new Dictionary<string, string>();
        }

        public void Save(string profileId, IReadOnlyDictionary<string, string> inputs)
        {
            var all = LoadAll();
            all[profileId] = new Dictionary<string, string>(inputs);

            try
            {
                Settings.Default.RenameProfileInputsJson = JsonSerializer.Serialize(all);
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                // Persistence is a nice-to-have; never let it fail the rename.
                prtDbg($"RenameInputsStore.Save failed: {ex.Message}");
            }
        }

        private static Dictionary<string, Dictionary<string, string>> LoadAll()
        {
            try
            {
                var json = Settings.Default.RenameProfileInputsJson;
                if (string.IsNullOrWhiteSpace(json)) return new();

                return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json)
                       ?? new();
            }
            catch (Exception ex)
            {
                prtDbg($"RenameInputsStore.LoadAll failed (using empty defaults): {ex.Message}");
                return new();
            }
        }
    }
}
