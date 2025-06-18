using ACSMCOMPONENTS25Lib;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SheetSetManager.SheetManager.Managers;
using SheetSetManager.SheetManager.Models;
using SheetSetManager.SheetManager.Views;
using SheetSetManager.Wrappers;

using System;
using System.Collections.ObjectModel;
using System.Linq;


namespace SheetSetManager.SheetManager.ViewModels
{
    internal partial class SheetSetViewModel : ObservableObject
    {
        private readonly Interop.SheetSetManager _sheetSetManager = new();

        public SheetsManager Sheets => _sheetSetManager.Sheets;        

        public SheetSetViewModel()
        {
            _sheetSetManager.LoadSheets();
        }        

        [RelayCommand]
        private void ApplyChanges()
        {
            //var modifiedSheets = Sheets.Where(s => s.IsEdited).ToList();
            //if (modifiedSheets.Count == 0) return;

            //var cw = new ApplyConfirmationWindow();
            //cw.ShowDialog();
            //if (!cw.IsConfirmed) return; // User cancelled

            //foreach (var sheet in Sheets.Where(s => s.IsEdited))
            //{
            //    var s = sheet.Oid.GetPersistObject() as AcSmSheet;
            //    if (s == null) continue;


            //}

            //HasPendingChanges = false;
        }

        [RelayCommand]
        private void ResetSheets()
        {
            //_sheetSetManager.LoadSheets(); // ✅ Reload sheets from database
            //HasPendingChanges = false;
        }        

        [RelayCommand]
        private void AddRevisionToSelected()
        {
            //SelectedSheet?.AddRevision();
        }

        [RelayCommand]
        private void RemoveRevisionFromSelected(RevisionModel revision)
        {
            //SelectedSheet?.RemoveRevision(revision);
        }

        public void OnCellEdit(SheetModel editedSheet, string bindingPath, string newValue)
        {

        }
    }
}
