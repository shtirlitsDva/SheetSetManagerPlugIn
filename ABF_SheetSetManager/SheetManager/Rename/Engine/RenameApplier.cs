using ACSMCOMPONENTS25Lib;

using SheetSetManager.SheetManager.Rename.Interop;

using System;
using System.Collections.Generic;

using static SheetSetManager.Utils;

namespace SheetSetManager.SheetManager.Rename.Engine
{
    /// <summary>
    /// Writes accepted preview rows back to SSM. Single Lock/Unlock pair; rolls back on failure.
    /// Pattern matches <see cref="SheetManager.ViewModels.SheetSetViewModel.ApplyChanges"/>.
    /// </summary>
    internal sealed class RenameApplier
    {
        public RenameApplyResult Apply(
            RenameSheetSource source,
            IReadOnlyList<RenamePreviewRow> rows)
        {
            int writtenSheets = 0;
            var errors = new List<string>();

            try
            {
                source.Lock();
                foreach (var row in rows)
                {
                    if (row.Status != RenameStatus.Matched) continue;

                    try
                    {
                        var sheet = source.ResolveSheet(row.OriginalIndex);
                        ApplyRow(sheet, row);
                        writtenSheets++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"[{row.OldNumber}] {ex.Message}");
                    }
                }
            }
            catch (Exception)
            {
                source.Unlock(commit: false);
                throw;
            }

            source.Unlock(commit: errors.Count == 0);

            var msg = errors.Count == 0
                ? $"Renamed {writtenSheets} sheet(s)."
                : $"Renamed {writtenSheets} sheet(s); {errors.Count} failed and changes were rolled back.";
            prtDbg(msg);

            return new RenameApplyResult(writtenSheets, errors);
        }

        private static void ApplyRow(AcSmSheet sheet, RenamePreviewRow row)
        {
            if (row.NewNumber is not null) sheet.SetNumber(row.NewNumber);
            if (row.NewTitle  is not null) sheet.SetTitle(row.NewTitle);

            if (row.PropertyChanges.Count > 0)
            {
                var bag = sheet.GetCustomPropertyBag();
                foreach (var change in row.PropertyChanges)
                {
                    var prop = bag.GetProperty(change.Name);
                    if (prop is null)
                        throw new InvalidOperationException(
                            $"Custom property '{change.Name}' not found on sheet.");
                    prop.SetValue(change.NewValue);
                }
            }
        }
    }

    public sealed record RenameApplyResult(int SheetsWritten, IReadOnlyList<string> Errors);
}
