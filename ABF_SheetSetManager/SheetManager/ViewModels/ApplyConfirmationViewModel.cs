using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.ViewModels
{
    internal partial class ApplyConfirmationViewModel : ObservableObject
    {
        public List<SheetModel> ModifiedSheets { get; private set; } = new();

        [ObservableProperty]
        private string _summaryText;

        public ApplyConfirmationViewModel() {}

        public void LoadModifiedSheets(List<SheetModel> modifiedSheets)
        {
            ModifiedSheets = modifiedSheets;

            if (modifiedSheets.Count == 1)
            {
                SummaryText = $"You are about to modify 1 sheet:\n{modifiedSheets[0].SheetNumber}";
            }
            else
            {
                SummaryText = $"You are about to modify {modifiedSheets.Count} sheets.";
            }

            // Collect changes for each sheet
            foreach (var sheet in modifiedSheets)
            {
                sheet.Changes = new List<string>();

                if (sheet.IsEdited)
                {
                    if (!string.IsNullOrEmpty(sheet.Title1)) sheet.Changes.Add($"Title1: {sheet.Title1}");
                    if (!string.IsNullOrEmpty(sheet.Title2)) sheet.Changes.Add($"Title2: {sheet.Title2}");
                    if (!string.IsNullOrEmpty(sheet.ApprovedBy)) sheet.Changes.Add($"ApprovedBy: {sheet.ApprovedBy}");
                    if (!string.IsNullOrEmpty(sheet.CheckedBy)) sheet.Changes.Add($"CheckedBy: {sheet.CheckedBy}");
                    if (!string.IsNullOrEmpty(sheet.DrawnBy)) sheet.Changes.Add($"DrawnBy: {sheet.DrawnBy}");
                    if (!string.IsNullOrEmpty(sheet.Scale)) sheet.Changes.Add($"Scale: {sheet.Scale}");
                }
            }
        }
    }
}
