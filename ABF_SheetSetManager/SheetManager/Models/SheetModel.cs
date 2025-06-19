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
    }
}
