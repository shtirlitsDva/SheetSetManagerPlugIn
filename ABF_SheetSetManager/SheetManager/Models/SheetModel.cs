using ACSMCOMPONENTS25Lib;
using CommunityToolkit.Mvvm.ComponentModel;
using SheetSetManager.SheetManager.Managers;

using System.Linq;

namespace SheetSetManager.SheetManager.Models
{
    internal partial class SheetModel : ObservableObject
    {
        public PropertyManager Properties { get; }
        public RevisionManager Revisions { get; }

        [ObservableProperty] private bool _isSelected = false;        

        public SheetModel(AcSmSheet comSheet)
        {
            Oid = comSheet.GetObjectId();

            Properties = new PropertyManager(comSheet);
            Revisions = new RevisionManager(comSheet);
        }

        public IAcSmObjectId Oid { get; }
        
        public void MarkAsEdited(string propertyName, string newValue)
        {
            //IsEdited = true;
            //Changes.Add($"{propertyName}: {newValue}");
        }

        public void AddRevision()
        {
            //char newRevLetter = Revisions.Count == 0 ? 'A' : (char)(Revisions[^1].RevisionLetter[0] + 1);
            //Revisions.Add(new RevisionModel { RevisionLetter = newRevLetter.ToString() });
        }

        public void RemoveRevision(RevisionModel rev)
        {
            Revisions.Remove(rev);
        }
    }
}
