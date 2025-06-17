using ACSMCOMPONENTS25Lib;

using SheetSetManager.SheetManager.Interfaces;
using SheetSetManager.Wrappers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Models
{
    internal class PropertyCustomModel : PropertyBase
    {
        private IAcSmObjectId Oid;

        public PropertyCustomModel(
            (string Name, AcSmCustomPropertyValue Value) property) :
            base(property.Name)
        {
            Value = property.Value.GetValue();
            Oid = property.Value.GetObjectId();
        }

        protected override void ImplementButDoNotCall_ApplyChange()
        {
            var customProperty = Oid.GetPersistObject() as AcSmCustomPropertyValue;
            if (customProperty == null)             
                throw new Exception("Custom property not found!");
            customProperty.SetValue(Value);
        }
    }
}
