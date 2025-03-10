using HandyControl.Controls;
using HandyControl.Themes;

using SheetSetManager.SheetManager.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
    }
}
