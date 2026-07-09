using HandyControl.Themes;

using SheetSetManager.SheetManager.Rename.Interop;
using SheetSetManager.SheetManager.Rename.Persistence;
using SheetSetManager.SheetManager.Rename.Profiles;
using SheetSetManager.SheetManager.Rename.ViewModels;

using System;

using Window = HandyControl.Controls.Window;

namespace SheetSetManager.SheetManager.Rename.Views
{
    /// <summary>
    /// One window for all three rename profiles. The window is intentionally thin —
    /// it owns no business logic; everything lives in <see cref="RenameSheetsViewModel"/>.
    /// </summary>
    internal partial class RenameSheetsWindow : Window
    {
        private readonly RenameSheetsViewModel _vm;

        public RenameSheetsWindow(RenameSheetsViewModel vm)
        {
            InitializeComponent();
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;

            _vm = vm;
            DataContext = _vm;
            _vm.CloseRequested += OnCloseRequested;
        }

        /// <summary>
        /// Convenience constructor used by the AutoCAD command handlers — wires the default
        /// profile provider, snapshots the open SSM database, and constructs the VM.
        /// </summary>
        public static RenameSheetsWindow CreateForOpenSheetSet(string? preselectedProfileId = null)
        {
            var provider = new DefaultRenameProfileProvider();
            var source = RenameSheetSource.Load();
            var store = new RenameInputsStore();
            var vm = new RenameSheetsViewModel(provider, source, store, preselectedProfileId);
            return new RenameSheetsWindow(vm);
        }

        private void OnCloseRequested(object? sender, EventArgs e)
        {
            DialogResult = _vm.Applied;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _vm.CloseRequested -= OnCloseRequested;
            base.OnClosed(e);
        }
    }
}
