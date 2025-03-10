using ACSMCOMPONENTS25Lib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABF_SheetSetManager.Wrappers
{
    internal struct AcSmProperty
    {
        public string Name { get; }
        public AcSmCustomPropertyValue Value { get; }

        public AcSmProperty(string name, AcSmCustomPropertyValue value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() => $"{Name}: {Value?.GetValue()}";
    }
}
