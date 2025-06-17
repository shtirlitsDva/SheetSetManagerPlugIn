using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SheetSetManager.SheetManager.Interfaces;
using SheetSetManager.Wrappers;

namespace SheetSetManager.SheetManager.Models
{
    internal class PropertyCustomModel : IProperty
    {
        public string Name { get; set; }
        public AcSmProperty Value { get; set; }
    }
}
