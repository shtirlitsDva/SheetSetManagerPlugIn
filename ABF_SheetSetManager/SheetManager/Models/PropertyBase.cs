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
            set
            {
                // Null and empty both mean "no value". WPF's edit TextBox commits ""
                // for a null cell, so entering an already-empty cell and leaving it
                // untouched must NOT be treated as a change.
                if (!string.Equals(_value ?? string.Empty, value ?? string.Empty, StringComparison.Ordinal))
                {
                    _value = value;
                    OnPropertyChanged();
                    ChangePending = true;
                }
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
