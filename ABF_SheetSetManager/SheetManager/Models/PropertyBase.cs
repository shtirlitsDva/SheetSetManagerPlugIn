using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Models
{
    public abstract class PropertyBase : ObservableObject, IProperty
    {
        public string Name { get; }
        private string? _value;
        public string? Value
        {
            get => _value;
            set { if (SetProperty(ref _value, value)) ChangePending = true; }
        }        

        public bool ChangePending { get; private set; } = false;
        public void ApplyChange()
        {
            if (!ChangePending) return;
            ImplementButDoNotCall_ApplyChange();
            ChangePending = false;
        }
        protected abstract void ImplementButDoNotCall_ApplyChange();
        
        public PropertyBase(string name)
        {
            Name = name;
        }
    }
}
