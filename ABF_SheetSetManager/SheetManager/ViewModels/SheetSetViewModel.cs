using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SheetSetManager.SheetManager.Models;
using SheetSetManager.Wrappers;
using SheetSetManager.SheetManager.Interop;
using ACSMCOMPONENTS25Lib;


namespace SheetSetManager.SheetManager.ViewModels
{
    internal partial class SheetSetViewModel : ObservableObject
    {
        public ObservableCollection<SheetModel> Sheets { get; } = new();

        [ObservableProperty] private SheetModel _selectedSheet;

        public SheetSetViewModel()
        {
            LoadSheets();
        }

        private void LoadSheets()
        {
            var sSet = Interop.SheetSetManager.GetCurrentSheetSet();

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
