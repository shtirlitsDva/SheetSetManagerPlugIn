using ACSMCOMPONENTS25Lib;

using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Interfaces;
using SheetSetManager.SheetManager.Models;
using SheetSetManager.Wrappers;

using System.Collections;
using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Managers
{
    public partial class PropertyManager : ObservableObject, IList<IProperty>
    {
        private readonly List<IProperty> _properties = new();
        private readonly Dictionary<string, IProperty> _propDict = new();

        [ObservableProperty] private IProperty _sheetNumber;
        [ObservableProperty] private IProperty _date;
        [ObservableProperty] private IProperty _title1;
        [ObservableProperty] private IProperty _title2;
        [ObservableProperty] private IProperty _approvedBy;
        [ObservableProperty] private IProperty _checkedBy;
        [ObservableProperty] private IProperty _drawnBy;
        [ObservableProperty] private IProperty _scale;

        private IAcSmObjectId _sheetId;
        public PropertyManager(AcSmSheet comSheet)
        {
            _sheetId = comSheet.GetObjectId();

            SheetNumber = new PropertySheetNumberModel(comSheet);
            Add(SheetNumber);

            var propertyBag = comSheet.GetCustomPropertyBag();

            var sheetProperties = new AcSmPropertyEnumerator(
                comSheet.GetCustomPropertyBag().GetPropertyEnumerator());

            foreach (var prop in sheetProperties)
            {
                var property = new PropertyCustomModel(prop);

                switch (prop.Name)
                {
                    case "1 Tegner": DrawnBy = property; Add(property); break;
                    case "Dato": Date = property; Add(property); break;
                    case "Emnelinje 1": Title1 = property; Add(property); break;
                    case "Emnelinje 2": Title2 = property; Add(property); break;
                    case "Godkendt": ApprovedBy = property; Add(property); break;
                    case "Kontrol": CheckedBy = property; Add(property); break;
                    case "Målestok ex 1:50": Scale = property; Add(property); break;
                }
            }
        }

        #region IList implementation
        public int Count => _properties.Count;

        public bool IsReadOnly => false;

        public IProperty this[string name]
        {
            get => _propDict[name];
            set => _propDict[name] = value;
        }

        public IProperty this[int index]
        {
            get => _properties[index];
            set => _properties[index] = value;
        }

        public int IndexOf(IProperty item)
        {
            return _properties.IndexOf(item);
        }

        public void Insert(int index, IProperty item)
        {
            _properties.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _properties.RemoveAt(index);
        }

        public void Add(IProperty item)
        {
            _properties.Add(item);
            _propDict.Add(item.Name, item);
        }

        public void Clear()
        {
            _properties.Clear();
        }

        public bool Contains(IProperty item)
        {
            return _properties.Contains(item);
        }

        public void CopyTo(IProperty[] array, int arrayIndex)
        {
            _properties.CopyTo(array, arrayIndex);
        }

        public bool Remove(IProperty item)
        {
            return _properties.Remove(item);
        }

        public IEnumerator<IProperty> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}