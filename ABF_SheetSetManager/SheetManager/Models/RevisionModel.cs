using ACSMCOMPONENTS25Lib;

using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Interfaces;

using System.Collections.Generic;
using System.ComponentModel;

namespace SheetSetManager.SheetManager.Models
{
    public partial class RevisionModel : ObservableObject
    {
        [ObservableProperty] private IProperty _revisionLetter;
        [ObservableProperty] private IProperty _date;
        [ObservableProperty] private IProperty _description;
        [ObservableProperty] private IProperty _approvedBy;
        [ObservableProperty] private IProperty _checkedBy;
        [ObservableProperty] private IProperty _drawnBy;

        public RevisionModel(List<(string Name, AcSmCustomPropertyValue Value)> list)
        {
            //Assume the list contains properties for this revision
            foreach (var prop in list)
            {
                var p = new PropertyCustomModel(prop);
                p.PropertyChanged += OnInnerValueChanged;
                if (prop.Name.Contains("Dato")) Date = p;
                else if (prop.Name.Contains("Emne")) Description = p;
                else if (prop.Name.Contains("Godkendt")) ApprovedBy = p;
                else if (prop.Name.Contains("Kontrol")) CheckedBy = p;
                else if (prop.Name.Contains("Tegner")) DrawnBy = p;
                else _revisionLetter = p; //Assume this is the revision letter
            }
        }

        private void OnInnerValueChanged(object? _, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IProperty.Value))
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }

        //Revision letter must not be checked for validity as it is always present
        public bool IsValid => !string.IsNullOrEmpty(RevisionLetter.Value) ||
                               !string.IsNullOrEmpty(Date.Value) ||
                               !string.IsNullOrEmpty(Description.Value) ||
                               !string.IsNullOrEmpty(ApprovedBy.Value) ||
                               !string.IsNullOrEmpty(CheckedBy.Value) ||
                               !string.IsNullOrEmpty(DrawnBy.Value);
    }
}