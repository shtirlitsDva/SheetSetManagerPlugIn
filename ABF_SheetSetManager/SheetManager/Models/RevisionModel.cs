using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace SheetSetManager.SheetManager.Models
{
    internal partial class RevisionModel : ObservableObject
    {
        [ObservableProperty]
        private string _revisionLetter;
        [ObservableProperty]
        private string _date;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private string _approvedBy;
        [ObservableProperty]
        private string _checkedBy;
        [ObservableProperty]
        private string _drawnBy;

        //Revision letter must not be checked for validity as it is always present
        public bool IsValid => //!string.IsNullOrEmpty(RevisionLetter) ||
                               !string.IsNullOrEmpty(Date) ||
                               !string.IsNullOrEmpty(Description) ||
                               !string.IsNullOrEmpty(ApprovedBy) ||
                               !string.IsNullOrEmpty(CheckedBy) ||
                               !string.IsNullOrEmpty(DrawnBy);
    }
}
