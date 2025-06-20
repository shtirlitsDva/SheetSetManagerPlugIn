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

using Window = HandyControl.Controls.Window;

namespace SheetSetManager.SheetManager.Views
{
    /// <summary>
    /// Interaction logic for RevisionDialog.xaml
    /// </summary>
    public partial class RevisionDialog : Window
    {
        RevisionTemplateViewModel vm = new();
        public RevisionDialog()
        {
            InitializeComponent();
            DataContext = vm;

            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
        }
        public RevisionTemplateModel Result => vm.BuildResult();
        private void SelectOnLoad(object sender, RoutedEventArgs e)
        {
            ((Control)sender).Focus();
            Keyboard.Focus((IInputElement)sender);
        }
    }
}
