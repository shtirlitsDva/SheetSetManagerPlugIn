using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SheetSetManager.SheetManager.Enums;
using SheetSetManager.SheetManager.Models;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;

namespace SheetSetManager.SheetManager.ViewModels
{
    public partial class RevisionTemplateViewModel : ObservableValidator
    {
        public RevisionTemplateViewModel() => ValidateAllProperties();

        [ObservableProperty, Required] private RevisionSequence sequence = RevisionSequence.Numeric;
        partial void OnSequenceChanged(RevisionSequence v)
        {
            ValidateProperty(v, nameof(Sequence));
            AcceptCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty, Required] private string date = "";
        partial void OnDateChanged(string v)
        {
            ValidateProperty(v, nameof(Date));
            AcceptCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty, Required] private string description = "";
        partial void OnDescriptionChanged(string v)
        {
            ValidateProperty(v, nameof(Description));
            AcceptCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty, Required] private string approvedBy = "";
        partial void OnApprovedByChanged(string v)
        {
            ValidateProperty(v, nameof(ApprovedBy));
            AcceptCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty, Required] private string checkedBy = "";
        partial void OnCheckedByChanged(string v)
        {
            ValidateProperty(v, nameof(CheckedBy));
            AcceptCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty, Required] private string drawnBy = "";
        partial void OnDrawnByChanged(string v)
        {
            ValidateProperty(v, nameof(DrawnBy));
            AcceptCommand.NotifyCanExecuteChanged();
        }

        // generated property: AcceptCommand
        [RelayCommand(CanExecute = nameof(CanAccept))]
        private void Accept(Window? w) 
        { 
            if (w != null && !HasErrors) w.DialogResult = true;
        }
        private bool CanAccept() => !HasErrors;

        public RevisionTemplateModel BuildResult() => new()
        {
            Sequence = this.Sequence,
            Date = this.Date,
            Description = this.Description,            
            ApprovedBy = this.ApprovedBy,
            DrawnBy = this.DrawnBy,
            CheckedBy = this.CheckedBy,
        };
    }
}
