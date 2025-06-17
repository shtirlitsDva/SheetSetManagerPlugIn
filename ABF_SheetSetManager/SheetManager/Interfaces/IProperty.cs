using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Interfaces
{
    internal interface IProperty
    {
        internal string Name { get; }
        internal string Value { get; set; }
        internal void ApplyChange();
    }
}
