using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SheetSetManager.SheetManager.Managers
{
    internal partial class SheetsManager :
        ObservableObject, 
        IList<SheetModel>,
        IList,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        private readonly ObservableCollection<SheetModel> _sheets = new();

        #region ▬▬▬ constructors ▬▬▬
        public SheetsManager() : this(Array.Empty<SheetModel>()) { }
        public SheetsManager(IEnumerable<SheetModel> initial)
        {
            _sheets = new ObservableCollection<SheetModel>(initial);

            // forward collection changes
            _sheets.CollectionChanged += (_, e) => CollectionChanged?.Invoke(this, e);

            // forward property changes that WPF controls rely on
            ((INotifyPropertyChanged)_sheets).PropertyChanged += (_, e) =>
            {
                // propagate only those the UI actually cares about
                if (e.PropertyName is "Count" or "Item[]")
                    OnPropertyChanged(e.PropertyName);
            };
        }
        #endregion

        #region ▬▬▬ sheet MVVM state ▬▬▬
        [ObservableProperty] private SheetModel? _selectedSheet;
        #endregion

        #region ▬▬▬ events ▬▬▬
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        #endregion

        #region ▬▬▬ IList<SheetModel> implementation ▬▬▬
        public SheetModel this[int index]
        {
            get => _sheets[index];
            set => _sheets[index] = value;
        }

        public int Count => _sheets.Count;
        public bool IsReadOnly => false;

        public void Add(SheetModel item) => _sheets.Add(item);
        public void Clear() => _sheets.Clear();
        public bool Contains(SheetModel item) => _sheets.Contains(item);
        public void CopyTo(SheetModel[] array, int idx) => _sheets.CopyTo(array, idx);
        public IEnumerator<SheetModel> GetEnumerator() => _sheets.GetEnumerator();
        public int IndexOf(SheetModel item) => _sheets.IndexOf(item);
        public void Insert(int idx, SheetModel item) => _sheets.Insert(idx, item);
        public bool Remove(SheetModel item) => _sheets.Remove(item);
        public void RemoveAt(int idx) => _sheets.RemoveAt(idx);
        IEnumerator IEnumerable.GetEnumerator() => _sheets.GetEnumerator();
        #endregion

        #region ▬▬▬ IList implementation ▬▬▬
        bool IList.IsFixedSize => false;
        bool IList.IsReadOnly => false;

        object? IList.this[int index]
        {
            get => _sheets[index];
            set => _sheets[index] = (SheetModel)value!;
        }

        int IList.Add(object? value)
        {
            _sheets.Add((SheetModel)value!);
            return _sheets.Count - 1;
        }

        void IList.Clear() => _sheets.Clear();
        bool IList.Contains(object? value) => _sheets.Contains((SheetModel)value!);
        int IList.IndexOf(object? value) => _sheets.IndexOf((SheetModel)value!);
        void IList.Insert(int i, object? value) => _sheets.Insert(i, (SheetModel)value!);
        void IList.Remove(object? value) => _sheets.Remove((SheetModel)value!);
        void IList.RemoveAt(int index) => _sheets.RemoveAt(index);

        // ICollection members required by IList
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => this;
        void ICollection.CopyTo(Array array, int index) =>
            ((ICollection)_sheets).CopyTo(array, index);
        #endregion
    }
}