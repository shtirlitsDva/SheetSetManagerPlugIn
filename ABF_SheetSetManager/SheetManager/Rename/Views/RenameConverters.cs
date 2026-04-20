using SheetSetManager.SheetManager.Rename.Engine;
using SheetSetManager.SheetManager.Rename.ViewModels;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SheetSetManager.SheetManager.Rename.Views
{
    /// <summary>
    /// Maps <see cref="RenameStatus"/> to a row-background brush so the user can spot errors.
    /// </summary>
    internal sealed class RenameStatusToBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush _matched  = Freeze(new SolidColorBrush(Color.FromRgb(0x35, 0x4D, 0x39)));
        private static readonly SolidColorBrush _noChange = Freeze(new SolidColorBrush(Color.FromRgb(0x3B, 0x44, 0x53)));
        private static readonly SolidColorBrush _error    = Freeze(new SolidColorBrush(Color.FromRgb(0x6B, 0x2E, 0x2E)));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is RenameStatus s
                ? s switch
                {
                    RenameStatus.Matched  => _matched,
                    RenameStatus.NoChange => _noChange,
                    RenameStatus.Error    => _error,
                    _                     => _noChange,
                }
                : (object)_noChange;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();

        private static SolidColorBrush Freeze(SolidColorBrush b) { b.Freeze(); return b; }
    }

    /// <summary>true → Collapsed, false → Visible. For "show validation error when invalid".</summary>
    internal sealed class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool b && b) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>Int → Visibility: 0 → Collapsed, &gt;0 → Visible.</summary>
    internal sealed class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is int n && n > 0) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Picks a DataTemplate based on the input field's runtime kind.
    /// Keeps the XAML free of per-profile knowledge — adding a new field type is one C# class
    /// + one DataTemplate + one selector branch.
    /// </summary>
    internal sealed class RenameInputFieldTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? TextTemplate { get; set; }
        public DataTemplate? ChoiceTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is RenameInputFieldViewModel vm)
                return vm.IsChoice ? ChoiceTemplate : TextTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}
