using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HandyControl.Controls;

using SheetSetManager.SheetManager.Models;
using SheetSetManager.SheetManager.ViewModels;

using Window = HandyControl.Controls.Window;

namespace SheetSetManager.SheetManager.Views
{
    /// <summary>
    /// Interaction logic for ApplyConfirmationWindow.xaml
    /// </summary>
    public partial class ApplyConfirmationWindow : Window
    {
        ApplyConfirmationViewModel vm = new();
        internal bool IsConfirmed { get; private set; }
        internal ApplyConfirmationWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }

        internal void SetModifiedSheets(List<SheetModel> modifiedSheets)
        {
            vm.LoadModifiedSheets(modifiedSheets);
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }
    }
}
