using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SheetSetManager.SheetManager.Rename.Engine;
using SheetSetManager.SheetManager.Rename.Interop;
using SheetSetManager.SheetManager.Rename.Persistence;
using SheetSetManager.SheetManager.Rename.Profiles;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SheetSetManager.SheetManager.Rename.ViewModels
{
    /// <summary>
    /// VM for the consolidated rename window. Holds the profile picker, the dynamic input
    /// fields for the selected profile, and the live preview rows. Engine runs are pure
    /// and cheap, so we re-run on every input edit; no debounce needed for ~hundreds of sheets.
    /// </summary>
    internal sealed partial class RenameSheetsViewModel : ObservableObject
    {
        private readonly IReadOnlyList<IRenameProfile> _profiles;
        private readonly RenameSheetSource _source;
        private readonly RenameEngine _engine = new();
        private readonly RenameInputsStore _inputsStore;

        public RenameSheetsViewModel(
            IRenameProfileProvider provider,
            RenameSheetSource source,
            RenameInputsStore inputsStore,
            string? preselectedProfileId = null)
        {
            _profiles = provider.GetProfiles();
            _source = source;
            _inputsStore = inputsStore;

            Profiles = _profiles;

            var initial = _profiles.FirstOrDefault(p => p.Id == preselectedProfileId)
                          ?? _profiles[0];
            SelectedProfile = initial;
        }

        #region Profile picker
        public IReadOnlyList<IRenameProfile> Profiles { get; }

        [ObservableProperty] private IRenameProfile _selectedProfile = null!;

        partial void OnSelectedProfileChanged(IRenameProfile value)
        {
            UnhookInputs();
            Inputs.Clear();

            var persisted = _inputsStore.Load(value.Id);
            foreach (var field in value.InputFields)
            {
                persisted.TryGetValue(field.Name, out var v);
                var fieldVm = new RenameInputFieldViewModel(field, v);
                fieldVm.PropertyChanged += OnInputFieldPropertyChanged;
                Inputs.Add(fieldVm);
            }

            RebuildPreview();
        }

        private void UnhookInputs()
        {
            foreach (var vm in Inputs)
                vm.PropertyChanged -= OnInputFieldPropertyChanged;
        }

        private void OnInputFieldPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RenameInputFieldViewModel.Value)
                || e.PropertyName == nameof(RenameInputFieldViewModel.IsValid))
            {
                RebuildPreview();
            }
        }
        #endregion

        #region Inputs + preview
        public ObservableCollection<RenameInputFieldViewModel> Inputs { get; } = new();
        public ObservableCollection<RenamePreviewRow> PreviewRows { get; } = new();

        [ObservableProperty] private int _matchedCount;
        [ObservableProperty] private int _noChangeCount;
        [ObservableProperty] private int _errorCount;

        public bool HasInputErrors => Inputs.Any(i => !i.IsValid);
        public bool HasPreviewErrors => ErrorCount > 0;

        public string StatusSummary =>
            $"{MatchedCount} to rename, {NoChangeCount} unchanged, {ErrorCount} error(s).";

        private void RebuildPreview()
        {
            OnPropertyChanged(nameof(HasInputErrors));

            PreviewRows.Clear();
            if (HasInputErrors)
            {
                MatchedCount = NoChangeCount = ErrorCount = 0;
                OnPropertyChanged(nameof(HasPreviewErrors));
                OnPropertyChanged(nameof(StatusSummary));
                ApplyCommand.NotifyCanExecuteChanged();
                return;
            }

            var inputDict = Inputs.ToDictionary(i => i.Name, i => i.Value ?? string.Empty);
            var rows = _engine.BuildPreview(SelectedProfile, inputDict, _source.Snapshots);
            foreach (var r in rows) PreviewRows.Add(r);

            MatchedCount  = rows.Count(r => r.Status == RenameStatus.Matched);
            NoChangeCount = rows.Count(r => r.Status == RenameStatus.NoChange);
            ErrorCount    = rows.Count(r => r.Status == RenameStatus.Error);

            OnPropertyChanged(nameof(HasPreviewErrors));
            OnPropertyChanged(nameof(StatusSummary));
            ApplyCommand.NotifyCanExecuteChanged();
        }
        #endregion

        #region Apply
        /// <summary>True after a successful Apply; the window uses this as DialogResult.</summary>
        public bool Applied { get; private set; }

        [RelayCommand(CanExecute = nameof(CanApply))]
        private void Apply()
        {
            // Defensive: never write while errors are present.
            if (!CanApply()) return;

            var applier = new RenameApplier();
            var result = applier.Apply(_source, PreviewRows);

            if (result.Errors.Count > 0)
            {
                System.Windows.MessageBox.Show(
                    "Rename failed and was rolled back:\n\n" + string.Join("\n", result.Errors),
                    "Rename sheets",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return;
            }

            // Persist user inputs only after a clean run.
            var toSave = Inputs.ToDictionary(i => i.Name, i => i.Value ?? string.Empty);
            _inputsStore.Save(SelectedProfile.Id, toSave);

            Applied = true;
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanApply() =>
            !HasInputErrors && !HasPreviewErrors && MatchedCount > 0;

        [RelayCommand]
        private void Cancel()
        {
            Applied = false;
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? CloseRequested;
        #endregion
    }
}
