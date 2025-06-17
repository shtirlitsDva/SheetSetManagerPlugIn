using ACSMCOMPONENTS25Lib;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SheetSetManager.SheetManager.Managers;
using System.Security.RightsManagement;

namespace SheetSetManager.SheetManager.Models
{
    internal partial class SheetModel : ObservableObject
    {
        public PropertyManager Properties { get; }
        public RevisionManager Revisions { get; }

        public SheetModel(AcSmSheet comSheet)
        {
            Oid = comSheet.GetObjectId();

            Properties = new PropertyManager(comSheet);
            Revisions = new RevisionManager(comSheet);
        }

        public IAcSmObjectId Oid { get; }
        

        public RevisionModel LatestRevision => Revisions.Count > 0 ? Revisions[^1] : null;

        public void MarkAsEdited(string propertyName, string newValue)
        {
            IsEdited = true;
            Changes.Add($"{propertyName}: {newValue}");
        }

        public void AddRevision()
        {
            char newRevLetter = Revisions.Count == 0 ? 'A' : (char)(Revisions[^1].RevisionLetter[0] + 1);
            Revisions.Add(new RevisionModel { RevisionLetter = newRevLetter.ToString() });
        }

        public void RemoveRevision(RevisionModel rev)
        {
            Revisions.Remove(rev);
        }
    }
}
