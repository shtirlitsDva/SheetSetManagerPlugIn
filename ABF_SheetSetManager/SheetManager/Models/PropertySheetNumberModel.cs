using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACSMCOMPONENTS25Lib;

using SheetSetManager.SheetManager.Interfaces;

namespace SheetSetManager.SheetManager.Models
{
    internal class PropertySheetNumberModel : IProperty
    {        
        private IAcSmObjectId _sheetId { get; }
        internal PropertySheetNumberModel(AcSmSheet comSheet)
        {
            _sheetId = comSheet.GetObjectId();
            
        }
    }
}
