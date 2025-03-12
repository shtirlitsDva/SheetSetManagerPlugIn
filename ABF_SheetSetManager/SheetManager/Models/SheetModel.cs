using ACSMCOMPONENTS25Lib;

using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Models
{
    internal partial class SheetModel : ObservableObject
    {
        [ObservableProperty] private bool _isSelected;
        [ObservableProperty] private bool _isEdited;
        [ObservableProperty] private string _sheetNumber;
        [ObservableProperty] private string _date;
        [ObservableProperty] private string _title1;
        [ObservableProperty] private string _title2;
        [ObservableProperty] private string _approvedBy;
        [ObservableProperty] private string _checkedBy;
        [ObservableProperty] private string _drawnBy;
        [ObservableProperty] private string _scale;
        private IAcSmObjectId _oid;
        public IAcSmObjectId Oid
        {
            get => _oid;
            set => _oid = value;
        }

        public List<string> Changes { get; set; } = new();

        public ObservableCollection<RevisionModel> Revisions { get; } = new();

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
