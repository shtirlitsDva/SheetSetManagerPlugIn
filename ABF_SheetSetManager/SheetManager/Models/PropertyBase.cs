using SheetSetManager.SheetManager.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Models
{
    internal abstract class PropertyBase : IProperty
    {
        internal string Name { get; }
        private string? _value;
        internal string? Value
        {
            get => _value;
            set { _value = value; ChangePending = true; }
        }
        internal bool ChangePending { get; set; } = false;
        internal abstract void ApplyChanges();
        protected void ApplyChangeBase()
        {
            ApplyChanges();
            ChangePending = false;
        }
        protected PropertyBase(string name)
        {
            Name = name;
        }
    }
}
