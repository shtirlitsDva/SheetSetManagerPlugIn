using ACSMCOMPONENTS25Lib;
using CommunityToolkit.Mvvm.ComponentModel;
using SheetSetManager.SheetManager.Managers;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace SheetSetManager.SheetManager.Models
{
    public partial class SheetModel : ObservableObject
    {
        public PropertyManager Properties { get; }
        public RevisionManager Revisions { get; }
        public List<SheetModel> AllSheetsOnDwg { get; set; } = new();

        [ObservableProperty] private bool _isSelected = false;        

        public SheetModel(AcSmSheet comSheet)
        {
            Oid = comSheet.GetObjectId();

            Properties = new PropertyManager(comSheet);
            Revisions = new RevisionManager(comSheet, this);
        }

        public IAcSmObjectId Oid { get; }        
    }
}
