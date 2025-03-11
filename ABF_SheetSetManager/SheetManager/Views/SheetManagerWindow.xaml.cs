using HandyControl.Controls;
using HandyControl.Themes;

using SheetSetManager.SheetManager.Models;
using SheetSetManager.SheetManager.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
    }
}
