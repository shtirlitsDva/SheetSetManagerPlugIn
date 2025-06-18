using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

//using TextBox = HandyControl.Controls.TextBox;

namespace SheetSetManager.SheetManager.Views
{
    public class CustomTextColumn : DataGridTextColumn
    {
        public string? TagPath { get; set; }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
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

        private void ApplyTagBinding(FrameworkElement fe)
        {
            if (TagPath is null) return;
            fe.SetBinding(FrameworkElement.TagProperty, new Binding(TagPath) { Mode = BindingMode.OneWay });
        }
    }
}
