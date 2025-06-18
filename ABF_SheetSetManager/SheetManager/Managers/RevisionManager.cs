using ACSMCOMPONENTS25Lib;

using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Models;
using SheetSetManager.Wrappers;

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace SheetSetManager.SheetManager.Managers
{
    public partial class RevisionManager :
        ObservableObject, IList<RevisionModel>,
        INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly ObservableCollection<RevisionModel> _revisions = new();

        public ICollectionView ValidRevisions { get; }

        [ObservableProperty]
        private RevisionModel? _latestRevision;

        private IAcSmObjectId _sheetId;

        public RevisionManager(AcSmSheet comSheet)
        {
            //Implement wpf stuff
            _revisions.CollectionChanged += (_, e) => CollectionChanged?.Invoke(this, e);
            ((INotifyPropertyChanged)_revisions).PropertyChanged += (_, e) =>
            {
                // propagate only those the UI actually cares about
                if (e.PropertyName is "Count" or "Item[]") OnPropertyChanged(e.PropertyName);
            };

            //Read the data
            _sheetId = comSheet.GetObjectId();

            var propertyBag = comSheet.GetCustomPropertyBag();

            var sheetProperties = new AcSmPropertyEnumerator(
                comSheet.GetCustomPropertyBag().GetPropertyEnumerator());

            var query = sheetProperties
                .Where(p => p.Name.StartsWith("Rev "))
                .GroupBy(p => p.Name.Split(' ')[1])
                .OrderBy(g => g.Key);

            foreach (var group in query)
            {
                var revision = new RevisionModel(group.ToList());
                AttachRevision(revision);
                this.Add(revision);
            }

            LatestRevision = _revisions.LastOrDefault(x => x.IsValid);

            ValidRevisions = CollectionViewSource.GetDefaultView(_revisions);
            ValidRevisions.Filter = r => ((RevisionModel)r).IsValid;
        }

        private void AttachRevision(RevisionModel rev) => rev.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(RevisionModel.IsValid)) ValidRevisions.Refresh();
        };

        #region ▬▬▬ events ▬▬▬
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        #endregion

        #region IList implementation
        public int Count => _revisions.Count;

        public bool IsReadOnly => false;

        public RevisionModel this[int index]
        {
            get => _revisions[index];
            set => _revisions[index] = value;
        }

        public int IndexOf(RevisionModel item)
        {
            return _revisions.IndexOf(item);
        }

        public void Insert(int index, RevisionModel item)
        {
            _revisions.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _revisions.RemoveAt(index);
        }

        public void Add(RevisionModel item)
        {
            _revisions.Add(item);
        }

        public void Clear()
        {
            _revisions.Clear();
        }

        public bool Contains(RevisionModel item)
        {
            return _revisions.Contains(item);
        }

        public void CopyTo(RevisionModel[] array, int arrayIndex)
        {
            _revisions.CopyTo(array, arrayIndex);
        }

        public bool Remove(RevisionModel item)
        {
            return _revisions.Remove(item);
        }

        public IEnumerator<RevisionModel> GetEnumerator()
        {
            return _revisions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}