using ACSMCOMPONENTS25Lib;

using SheetSetManager.SheetManager.Managers;
using SheetSetManager.SheetManager.Models;
using SheetSetManager.Wrappers;

using System;

using static SheetSetManager.Utils;

namespace SheetSetManager.SheetManager.Interop
{
    internal class SheetSetManager
    {
        internal static AcSmDatabase? _db = null;
        private SheetsManager _sheets = new();

        internal void InitializeDatabase()
        {
            #region Init SSM database
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
            _currentDatabase = ssDb; 
            #endregion
            #endregion

            if (_currentDatabase == null)
            {
                prtDbg("No database is open! Open one and only one database (.dst file)!");
                throw new Exception("No database is open! Open one and only one database (.dst file)!");
            }

            prtDbg("Sheet Set Manager database initialized successfully.");

            var sSet = _currentDatabase.GetSheetSet();
            var ssEnum = new AcSmComEnumerator(sSet.GetSheetEnumerator());

            foreach (var ssComp in ssEnum)
            {
                if (ssComp.GetTypeName() != "AcSmSubset") continue;
                AcSmSubset subset = (AcSmSubset)ssComp;
                var subsetEnum = new AcSmComEnumerator(subset.GetSheetEnumerator());

                foreach (var sbsComp in subsetEnum)
                {
                    if (sbsComp.GetTypeName() != "AcSmSheet") continue;
                    AcSmSheet sheet = (AcSmSheet)sbsComp;                    

                    var sheetModel = new SheetModel(sheet);
                                  
                    _sheets.Add(sheetModel);
                }
            }
        }
    }
}