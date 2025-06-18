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
                SummaryText = $"You are about to modify 1 sheet:\n{modifiedSheets[0].Properties.SheetNumber}";
            }
            else
            {
                SummaryText = $"You are about to modify {modifiedSheets.Count} sheets.";
            }

            
        }
    }
}
