using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

//using TextBox = HandyControl.Controls.TextBox;

namespace SheetSetManager.SheetManager.Views
{
    public class CustomTextColumn : DataGridTextColumn
    {
        public string? TagPath { get; set; }

        // Highlight painted on a cell while it holds an un-applied (pending) edit.
        private static readonly SolidColorBrush PendingBrush =
            new(Color.FromRgb(0xFF, 0xEB, 0x3B)); // yellow

        private bool _pendingStyleBuilt;

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            BuildPendingHighlight();
            var fe = base.GenerateElement(cell, dataItem);
            ApplyTagBinding(fe);
            return fe;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var fe = base.GenerateEditingElement(cell, dataItem);
            ApplyTagBinding(fe);
            return fe;
        }

        /// <summary>
        /// Adds a <see cref="DataTrigger"/> to the column's <see cref="DataGridTextColumn.ElementStyle"/>
        /// that paints the cell yellow whenever the bound property has a pending change.
        /// The dirty flag lives next to the value: a cell bound to "….Value" exposes
        /// its pending state at "….ChangePending".
        /// </summary>
        private void BuildPendingHighlight()
        {
            if (_pendingStyleBuilt) return;
            _pendingStyleBuilt = true;

            if (Binding is not Binding valueBinding) return;
            var valuePath = valueBinding.Path?.Path;
            if (string.IsNullOrEmpty(valuePath) || !valuePath.EndsWith(".Value")) return;

            var pendingPath = valuePath.Substring(0, valuePath.Length - ".Value".Length)
                              + ".ChangePending";

            var style = new Style(typeof(TextBlock), ElementStyle);
            var trigger = new DataTrigger
            {
                Binding = new Binding(pendingPath) { Mode = BindingMode.OneWay },
                Value = true
            };
            trigger.Setters.Add(new Setter(TextBlock.BackgroundProperty, PendingBrush));
            trigger.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Black));
            style.Triggers.Add(trigger);

            ElementStyle = style;
        }

        private void ApplyTagBinding(FrameworkElement fe)
        {
            if (TagPath is null) return;
            fe.SetBinding(FrameworkElement.TagProperty, new Binding(TagPath) { Mode = BindingMode.OneWay });
        }
    }
}
