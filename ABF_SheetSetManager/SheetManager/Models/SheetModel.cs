using ACSMCOMPONENTS25Lib;

using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Interfaces;
using SheetSetManager.Wrappers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Models
{
    internal partial class SheetModel : ObservableObject
    {
        [ObservableProperty] private bool _isSelected = false;
        [ObservableProperty] private bool _isEdited = false;
        [ObservableProperty] private IProperty _sheetNumber;
        [ObservableProperty] private IProperty _date;
        [ObservableProperty] private IProperty _title1;
        [ObservableProperty] private IProperty _title2;
        [ObservableProperty] private IProperty _approvedBy;
        [ObservableProperty] private IProperty _checkedBy;
        [ObservableProperty] private IProperty _drawnBy;
        [ObservableProperty] private IProperty _scale;
        
        public SheetModel(AcSmSheet comSheet)
        {
            Oid = comSheet.GetObjectId();

            var sheetProperties = new AcSmPropertyEnumerator(
                comSheet.GetCustomPropertyBag().GetPropertyEnumerator());

            foreach (var prop in sheetProperties)
            {
                switch (prop.Name)
                {
                    case "1 Tegner": DrawnBy = prop.Value.GetValue(); break;
                    case "Dato": Date = prop.Value.GetValue(); break;
                    case "Emnelinje 1": Title1 = prop.Value.GetValue(); break;
                    case "Emnelinje 2": Title2 = prop.Value.GetValue(); break;
                    case "Godkendt": ApprovedBy = prop.Value.GetValue(); break;
                    case "Gælder for": Scale = prop.Value.GetValue(); break;
                    case "Kontrol": CheckedBy = prop.Value.GetValue(); break;
                    case "Målestok ex 1:50": Scale = prop.Value.GetValue(); break;
                }
            }
        }

        public IAcSmObjectId Oid { get; }        

        public List<string> Changes { get; set; } = new();

        public ObservableCollection<RevisionModel> Revisions { get; } = new();

        public RevisionModel LatestRevision => Revisions.Count > 0 ? Revisions[^1] : null;

        public void MarkAsEdited(string propertyName, string newValue)
        {
            IsEdited = true;
            Changes.Add($"{propertyName}: {newValue}");
        }

        public void AddRevision()
        {
            char newRevLetter = Revisions.Count == 0 ? 'A' : (char)(Revisions[^1].RevisionLetter[0] + 1);
            Revisions.Add(new RevisionModel { RevisionLetter = newRevLetter.ToString() });
        }

        public void RemoveRevision(RevisionModel rev)
        {
            Revisions.Remove(rev);
        }
    }
}
