using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Rename.Profiles;

namespace SheetSetManager.SheetManager.Rename.ViewModels
{
    /// <summary>
    /// Wraps one <see cref="RenameInputField"/> with its current edited value and live validation.
    /// Notifies its parent VM via <see cref="OnPropertyChanged"/> on Value changes so the
    /// preview can re-run.
    /// </summary>
    internal sealed partial class RenameInputFieldViewModel : ObservableObject
    {
        public RenameInputFieldViewModel(RenameInputField field, string? initialValue)
        {
            Field = field;
            _value = initialValue ?? field.DefaultValue;
            Revalidate();
        }

        public RenameInputField Field { get; }

        public string Name  => Field.Name;
        public string Label => Field.Label;

        /// <summary>True when this field is a closed-set choice (renders as ComboBox).</summary>
        public bool IsChoice => Field is ChoiceRenameInputField;

        public System.Collections.Generic.IReadOnlyList<RenameChoice>? Choices =>
            (Field as ChoiceRenameInputField)?.Choices;

        private string? _value;
        public string? Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))
                {
                    Revalidate();
                }
            }
        }

        [ObservableProperty] private bool _isValid;
        [ObservableProperty] private string? _errorMessage;

        private void Revalidate()
        {
            var v = Field.Validate(_value);
            IsValid = v.Ok;
            ErrorMessage = v.Error;
        }
    }
}
