using Autodesk.AutoCAD.DatabaseServices;

using SheetSetManager.SheetManager.Models;

using System;
using System.Collections.Generic;

using static SheetSetManager.Utils;

namespace SheetSetManager.SheetManager.Interop
{
    internal static class AcOnOffRevisionLayers
    {
        internal static void Apply(List<RevisionApplicationModel> applicationModels)
        {
            foreach (RevisionApplicationModel model in applicationModels)
            {
                using Database sheetDb = new Database(false, true);
                sheetDb.ReadDwgFile(model.DatabaseFileName,
                        System.IO.FileShare.ReadWrite, false, "");
                using Transaction tx = sheetDb.TransactionManager.StartTransaction();

                try
                {
                    prdDbg($"Processing:\n{model.DatabaseFileName}");
                    foreach (RevisionApplicationAction action in model.Actions)
                    {
                        action.ApplyAction(sheetDb);
                    }
                }
                catch (Exception ex)
                {
                    prdDbg(ex);
                    tx.Abort();
                    sheetDb.Dispose();
                    throw;
                }
                sheetDb.SaveAs(model.DatabaseFileName, true, DwgVersion.Newest, null);
                tx.Commit();
            }
        }
    }
}