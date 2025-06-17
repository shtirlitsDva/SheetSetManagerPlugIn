using ACSMCOMPONENTS25Lib;

using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Interfaces;
using SheetSetManager.SheetManager.Models;
using SheetSetManager.Wrappers;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SheetSetManager.SheetManager.Managers
{
    public partial class RevisionManager : ObservableObject, IList<RevisionModel>
    {
        private readonly ObservableCollection<RevisionModel> _revisions = new();
        
        [ObservableProperty]
        private RevisionModel? _latestRevision;

        private IAcSmObjectId _sheetId;

        public RevisionManager(AcSmSheet comSheet)
        {
            _sheetId = comSheet.GetObjectId();

            var propertyBag = comSheet.GetCustomPropertyBag();

            var sheetProperties = new AcSmPropertyEnumerator(
                comSheet.GetCustomPropertyBag().GetPropertyEnumerator());

            var query = sheetProperties
                .Where(p => p.Name.StartsWith("Rev "))
                .GroupBy(p => p.Name.Split(' ')[1])
                .OrderBy(g => g.Key);

            List<RevisionModel> revisions = new List<RevisionModel>();
            foreach (var group in query)
            {
                var revision = new RevisionModel(group.ToList());
                revisions.Add(revision);                
            }

            // Ensure the revisions are sorted by revision letter
            _revisions = new(revisions.OrderBy(x => x.RevisionLetter.Value));

            LatestRevision = _revisions.LastOrDefault(x => x.IsValid);
        }
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