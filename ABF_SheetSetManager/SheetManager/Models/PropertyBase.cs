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
            set { 
                if (SetProperty(ref _value, value)) ChangePending = true;
                OnPropertyChanged(nameof(ChangePending));
            }
        }        

        public bool ChangePending { get; private set; } = false;
        public void ApplyChange()
        {
            if (!ChangePending) return;
            ActualApplyChange();
            ChangePending = false;
            OnPropertyChanged(nameof(ChangePending));
        }
        protected abstract void ActualApplyChange();
        
        public PropertyBase(string name, string? initialValue)
        {
            Name = name;
            _value = initialValue;
        }
    }
}
