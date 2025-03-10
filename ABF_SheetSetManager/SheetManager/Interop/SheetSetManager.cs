using ACSMCOMPONENTS25Lib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static ABF_SheetSetManager.Utils;

namespace ABF_SheetSetManager.SheetManager.Interop
{
    internal class SheetSetManager
    {
        internal static AcSmSheetSet GetCurrentSheetSet()
        {
            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();
            // Get the loaded databases 
            IAcSmEnumDatabase enumDatabase = sheetSetManager.GetDatabaseEnumerator();

            #region Safeguarding for mulitple open databases
            //Safeguarding for multiple open databases
            int dbCount = 0;
            IAcSmPersist item = enumDatabase.Next();
            while (item != null)
            {
                dbCount++;
                item = enumDatabase.Next();
            }
            if (dbCount > 1)
            {
                prtDbg("Multiple databases open! Only one database must be open (.dst file)!");
                throw new Exception("Multiple databases open! Only one database must be open (.dst file)!");
            }
            if (dbCount < 1)
            {
                prtDbg("No database is open! Open one and only one database (.dst file)!");
                throw new Exception("No database is open! Open one and only one database (.dst file)!");
            }
            #endregion

            #region Get the Sheet Set
            enumDatabase.Reset();
            item = enumDatabase.Next();
            AcSmDatabase ssDb = item.GetDatabase();
            AcSmSheetSet sSet = ssDb.GetSheetSet();
            return sSet;
            #endregion
        }
    }
}