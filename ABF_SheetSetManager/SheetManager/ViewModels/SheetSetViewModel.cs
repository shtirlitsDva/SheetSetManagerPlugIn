using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SheetSetManager.SheetManager.Models;
using SheetSetManager.Wrappers;
using SheetSetManager.SheetManager.Interop;
using ACSMCOMPONENTS25Lib;

using SheetSetManager.SheetManager.Views;


namespace SheetSetManager.SheetManager.ViewModels
{
    internal partial class SheetSetViewModel : ObservableObject
    {
        public ObservableCollection<SheetModel> Sheets { get; } = new();

        [ObservableProperty] private SheetModel _selectedSheet;
        [ObservableProperty] private bool _hasPendingChanges;
        private AcSmDatabase _currentDatabase;

        public SheetSetViewModel()
        {
            LoadSheets();
        }

        private void LoadSheets()
        {
            Sheets.Clear();

            var ssDb = Interop.SheetSetManager.GetCurrentDatabase();
            _currentDatabase = ssDb;
            if (ssDb == null) return;

            var sSet = ssDb.GetSheetSet();
            var ssEnum = new AcSmComEnumerator(sSet.GetSheetEnumerator());

            foreach (var ssComp in ssEnum)
            {
                if (ssComp.GetTypeName() != "AcSmSubset") continue;
                AcSmSubset subset = (AcSmSubset)ssComp;

                var subsetEnum = new AcSmComEnumerator(subset.GetSheetEnumerator());

                foreach (var sbsComp in subsetEnum)
                {
                    if (sbsComp.GetTypeName() != "AcSmSheet") continue;
                    AcSmSheet sheet = (AcSmSheet)sbsComp;

                    var sheetProperties = new AcSmPropertyEnumerator(
                        sheet.GetCustomPropertyBag().GetPropertyEnumerator());

                    var model = new SheetModel();
                    model.Oid = sheet.GetObjectId(); //Stable reference to the object
                    model.SheetNumber = sheet.GetNumber();
                    foreach (var prop in sheetProperties)
                    {
                        switch (prop.Name)
                        {
                            case "1 Tegner": model.DrawnBy = prop.Value.GetValue(); break;
                            case "Dato": model.Date = prop.Value.GetValue(); break;
                            case "Emnelinje 1": model.Title1 = prop.Value.GetValue(); break;
                            case "Emnelinje 2": model.Title2 = prop.Value.GetValue(); break;
                            case "Godkendt": model.ApprovedBy = prop.Value.GetValue(); break;
                            case "Gælder for": model.Scale = prop.Value.GetValue(); break;
                            case "Kontrol": model.CheckedBy = prop.Value.GetValue(); break;
                            case "Målestok ex 1:50": model.Scale = prop.Value.GetValue(); break;
                        }

                        if (prop.Name.StartsWith("Rev "))
                        {
                            string revLetter = prop.Name.Split(' ')[1];
                            var existingRev = model.Revisions.FirstOrDefault(r => r.RevisionLetter == revLetter);
                            if (existingRev == null)
                            {
                                existingRev = new RevisionModel { RevisionLetter = revLetter };
                                model.Revisions.Add(existingRev);
                            }

                            if (prop.Name.Contains("Dato")) existingRev.Date = prop.Value.GetValue();
                            if (prop.Name.Contains("Emne")) existingRev.Description = prop.Value.GetValue();
                            if (prop.Name.Contains("Godkendt")) existingRev.ApprovedBy = prop.Value.GetValue();
                            if (prop.Name.Contains("Kontrol")) existingRev.CheckedBy = prop.Value.GetValue();
                            if (prop.Name.Contains("Tegner")) existingRev.DrawnBy = prop.Value.GetValue();
                        }
                    }

                    var validRevisions = model.Revisions.Where(r => r.IsValid).ToList();

                    model.Revisions.Clear();
                    foreach (var rev in validRevisions)
                    {
                        model.Revisions.Add(rev);
                    }

                    Sheets.Add(model);
                }
            }
        }

        [RelayCommand]
        private void ApplyChanges()
        {
            var modifiedSheets = Sheets.Where(s => s.IsEdited).ToList();
            if (modifiedSheets.Count == 0) return;

            var cw = new ApplyConfirmationWindow();

            foreach (var sheet in Sheets.Where(s => s.IsEdited))
            {
                var s = sheet.Oid.GetPersistObject() as AcSmSheet;
                if (s == null) continue;
                

            }

            HasPendingChanges = false;
        }

        [RelayCommand]
        private void ResetSheets()
        {
            LoadSheets(); // ✅ Reload sheets from database
            HasPendingChanges = false;
        }

        public void OnCellEdit(SheetModel sheet, string propertyName, string newValue)
        {
            sheet.MarkAsEdited(propertyName, newValue);
            HasPendingChanges = true;

            // ✅ Propagate changes to all selected sheets
            foreach (var s in Sheets.Where(s => s.IsSelected))
            {
                switch (propertyName)
                {
                    case "SheetNumber": s.SheetNumber = newValue; break;
                    case "Date": s.Date = newValue; break;
                    case "Title1": s.Title1 = newValue; break;
                    case "Title2": s.Title2 = newValue; break;
                    case "ApprovedBy": s.ApprovedBy = newValue; break;
                    case "CheckedBy": s.CheckedBy = newValue; break;
                    case "DrawnBy": s.DrawnBy = newValue; break;
                    case "Scale": s.Scale = newValue; break;
                }

                if (sheet.Oid != s.Oid)
                    s.MarkAsEdited(propertyName, newValue);
            }
        }

        [RelayCommand]
        private void AddRevisionToSelected()
        {
            SelectedSheet?.AddRevision();
        }

        [RelayCommand]
        private void RemoveRevisionFromSelected(RevisionModel revision)
        {
            SelectedSheet?.RemoveRevision(revision);
        }
    }
}
