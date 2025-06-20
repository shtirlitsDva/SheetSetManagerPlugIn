using Autodesk.AutoCAD.DatabaseServices;

using SheetSetManager.SheetManager.Enums;

namespace SheetSetManager.SheetManager.Models
{
    public readonly record struct RevisionApplicationAction(
        RevisionId revisionId,
        RevisionAction action)
    {
        public void ApplyAction(Database db)
        {
            string overskrifter = "Revisionsoverskrifter";

            string layer = "REV." + revisionId.ToString().Substring(3);

            Transaction tx = db.TransactionManager.TopTransaction;

            var lt = (LayerTable)tx.GetObject(db.LayerTableId, OpenMode.ForRead);

            if (!lt.Has(layer)) return;
            var ltr = (LayerTableRecord)tx.GetObject(lt[layer], OpenMode.ForWrite);

            switch (action)
            {
                case RevisionAction.Undetermined:
                    break;
                case RevisionAction.On:
                    ltr.IsFrozen = false;
                    ltr.IsOff = false;
                    break;
                case RevisionAction.Off:
                    ltr.IsFrozen = true;
                    ltr.IsOff = true;
                    break;
            }

            if (revisionId == RevisionId.RevA)
            {
                if (!lt.Has(overskrifter)) return;
                ltr = (LayerTableRecord)tx.GetObject(lt[overskrifter], OpenMode.ForWrite);

                switch (action)
                {
                    case RevisionAction.Undetermined:
                        break;
                    case RevisionAction.On:
                        ltr.IsFrozen = false;
                        ltr.IsOff = false;
                        break;
                    case RevisionAction.Off:
                        ltr.IsFrozen = true;
                        ltr.IsOff = true;
                        break;
                }
            }
        }
    }
}