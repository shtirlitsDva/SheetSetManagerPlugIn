using HandyControl.Themes;

using SheetSetManager.SheetManager.Models;
using SheetSetManager.SheetManager.ViewModels;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

//using TextBox = HandyControl.Controls.TextBox;
using Window = HandyControl.Controls.Window;

namespace SheetSetManager.SheetManager.Views
{
    /// <summary>
    /// Interaction logic for SheetManagerWindow.xaml
    /// </summary>
    public partial class SheetManagerWindow : Window
    {
        SheetSetViewModel vm = new();

        public SheetManagerWindow()
        {
            InitializeComponent();
            DataContext = vm;

            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
        }

        private void CheckBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Prevent row expansion when clicking the checkbox
            e.Handled = true;

            // Toggle the checkbox manually
            if (sender is CheckBox checkBox)
            {
                checkBox.IsChecked = !checkBox.IsChecked;
            }
        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) // Check if Spacebar is pressed
            {
                if (sender is DataGrid dataGrid && dataGrid.SelectedItem is SheetModel selectedSheet)
                {
                    selectedSheet.IsSelected = !selectedSheet.IsSelected; // Toggle checkbox state
                }
            }
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {

        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox selectAllCheckBox && DataContext is SheetSetViewModel viewModel)
            {
                foreach (var sheet in viewModel.Sheets)
                {
                    sheet.IsSelected = !sheet.IsSelected;
                }
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column is CustomTextColumn col &&
            !string.IsNullOrEmpty(col.TagPath) &&
            e.EditingElement is TextBox tb &&
            DataContext is SheetSetViewModel vm)
            {
                vm.BroadcastEdit(e.Row.Item, col.TagPath, tb.Text);
            }
        }
    }
}
