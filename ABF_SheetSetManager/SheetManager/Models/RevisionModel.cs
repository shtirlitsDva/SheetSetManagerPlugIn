using ACSMCOMPONENTS25Lib;

using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var property = new PropertyCustomModel(prop);
                if (prop.Name.Contains("Dato")) Date = property;
                else if (prop.Name.Contains("Emne")) Description = property;
                else if (prop.Name.Contains("Godkendt")) ApprovedBy = property;
                else if (prop.Name.Contains("Kontrol")) CheckedBy = property;
                else if (prop.Name.Contains("Tegner")) DrawnBy = property;
                else _revisionLetter = property; //Assume this is the revision letter
            }            
        }

        //Revision letter must not be checked for validity as it is always present
        public bool IsValid => //!string.IsNullOrEmpty(RevisionLetter) ||
                               !string.IsNullOrEmpty(Date.Value) ||
                               !string.IsNullOrEmpty(Description.Value) ||
                               !string.IsNullOrEmpty(ApprovedBy.Value) ||
                               !string.IsNullOrEmpty(CheckedBy.Value) ||
                               !string.IsNullOrEmpty(DrawnBy.Value);
    }
}