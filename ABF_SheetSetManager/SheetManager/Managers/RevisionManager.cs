using ACSMCOMPONENTS25Lib;

using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Enums;
using SheetSetManager.SheetManager.Models;
using SheetSetManager.Wrappers;
using static SheetSetManager.Utils;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace SheetSetManager.SheetManager.Managers
{
    public partial class RevisionManager :
        ObservableObject,
        IList<RevisionModel>,
        IList,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        private readonly ObservableCollection<RevisionModel> _all = new();
        private readonly ObservableCollection<RevisionModel> _valid = new();
        public ObservableCollection<RevisionModel> AllRevisions => _all;

        [ObservableProperty] private RevisionModel? _latestRevision;

        private IAcSmObjectId _sheetId;

        public RevisionManager(AcSmSheet comSheet)
        {
            //Implement wpf stuff
            _valid.CollectionChanged += (_, e) => CollectionChanged?.Invoke(this, e);
            ((INotifyPropertyChanged)_valid).PropertyChanged += (_, e) =>
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
                _all.Add(revision);
                if (revision.IsValid) _valid.Add(revision);
            }

            LatestRevision = _valid.LastOrDefault();            
        }

        internal void AddNextRevision(RevisionTemplateModel template)
        {
            var revisionToFillOut = _all.First(
                x => !_valid.Contains(x));

            string? revLetter = LatestRevision?.RevisionLetter.Value;
            revLetter = DetermineNextRevisionLetter(
                revLetter, template.Sequence);

            revisionToFillOut.RevisionLetter.Value = revLetter;
            revisionToFillOut.Date.Value = template.Date;
            revisionToFillOut.Description.Value = template.Description;
            revisionToFillOut.ApprovedBy.Value = template.ApprovedBy;
            revisionToFillOut.CheckedBy.Value = template.CheckedBy;
            revisionToFillOut.DrawnBy.Value = template.DrawnBy;

            revisionToFillOut.ChangePending = true;

            _valid.Add(revisionToFillOut);
            LatestRevision = revisionToFillOut;
        }

        private static string DetermineNextRevisionLetter(
            string? previousLetter, RevisionSequence selectedSequence)
        {
            return selectedSequence switch
            {
                RevisionSequence.Numeric => NextNumeric(previousLetter),
                RevisionSequence.Alphabetic => NextAlpha(previousLetter),
                _ => throw new Exception("Unsupported RevisionSequence!"),
            };
        }

        internal void RemoveLatestRevision()
        {
            if (LatestRevision == null) return;
            LatestRevision.Blank();
            LatestRevision.ChangePending = true;
            _valid.Remove(LatestRevision);
            LatestRevision = _valid.LastOrDefault();
        }

        #region ▬▬▬ events ▬▬▬
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        #endregion

        #region ▬▬▬ IList<RevisionModel> implementation ▬▬▬ 
        public int Count => _valid.Count;

        public bool IsReadOnly => false;

        public RevisionModel this[int index]
        {
            get => _valid[index];
            set => _valid[index] = value;
        }

        public int IndexOf(RevisionModel item)
        {
            return _valid.IndexOf(item);
        }

        public void Insert(int index, RevisionModel item)
        {
            _valid.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _valid.RemoveAt(index);
        }

        public void Add(RevisionModel item)
        {
            _valid.Add(item);
        }

        public void Clear()
        {
            _valid.Clear();
        }

        public bool Contains(RevisionModel item)
        {
            return _valid.Contains(item);
        }

        public void CopyTo(RevisionModel[] array, int arrayIndex)
        {
            _valid.CopyTo(array, arrayIndex);
        }

        public bool Remove(RevisionModel item)
        {
            return _valid.Remove(item);
        }

        public IEnumerator<RevisionModel> GetEnumerator()
        {
            return _valid.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region ▬▬▬ IList implementation ▬▬▬
        bool IList.IsFixedSize => false;
        bool IList.IsReadOnly => false;
        object? IList.this[int index]
        {
            get => _valid[index];
            set => _valid[index] = (RevisionModel)value!;
        }
        int IList.Add(object? value)
        {
            _valid.Add((RevisionModel)value!);
            return _valid.Count - 1;
        }
        void IList.Clear() => _valid.Clear();
        bool IList.Contains(object? value) => _valid.Contains((RevisionModel)value!);
        int IList.IndexOf(object? value) => _valid.IndexOf((RevisionModel)value!);
        void IList.Insert(int i, object? value) => _valid.Insert(i, (RevisionModel)value!);
        void IList.Remove(object? value) => _valid.Remove((RevisionModel)value!);
        void IList.RemoveAt(int index) => _valid.RemoveAt(index);

        // ICollection members required by IList
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => this;
        void ICollection.CopyTo(Array array, int index) =>
            ((ICollection)_valid).CopyTo(array, index);
        #endregion
    }
}