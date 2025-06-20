using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACSMCOMPONENTS25Lib;

using SheetSetManager.SheetManager.Interfaces;
using SheetSetManager.SheetManager.Interop;

namespace SheetSetManager.SheetManager.Models
{
    internal class PropertySheetNumberModel : PropertyBase
    {        
        private IAcSmObjectId _sheetId { get; }
        internal PropertySheetNumberModel(AcSmSheet comSheet) : base("Number")
        {
            _sheetId = comSheet.GetObjectId();
            Value = comSheet.GetNumber();
        }
        protected override void ActualApplyChange()
        {
            var sheet = _sheetId.GetPersistObject() as AcSmSheet;
            if (sheet == null) throw new Exception("Sheet not found!");

            sheet.SetNumber(Value);
        }
    }
}
