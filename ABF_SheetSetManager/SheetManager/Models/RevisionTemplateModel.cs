using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Models
{
    public partial class RevisionTemplateModel : ObservableObject
    {
        [ObservableProperty] private RevisionSequence _sequence;
        [ObservableProperty] private string _date;
        [ObservableProperty] private string _description;
        [ObservableProperty] private string _approvedBy;
        [ObservableProperty] private string _checkedBy;
        [ObservableProperty] private string _drawnBy;        
    }
}
