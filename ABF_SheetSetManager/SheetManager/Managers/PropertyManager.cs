using SheetSetManager.SheetManager.Interfaces;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Managers
{
    internal class PropertyManager : IList<IProperty>
    {
        private readonly List<IProperty> _properties = new List<IProperty>();

        public IProperty this[int index]
        {
            get => _properties[index];
            set => _properties[index] = value;
        }

        public int Count => _properties.Count;

        public bool IsReadOnly => false;

        public void Add(IProperty item)
        {
            _properties.Add(item);
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

        public IEnumerator<IProperty> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(IProperty item)
        {
            return _properties.IndexOf(item);
        }

        public void Insert(int index, IProperty item)
        {
            _properties.Insert(index, item);
        }

        public bool Remove(IProperty item)
        {
            return _properties.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _properties.RemoveAt(index);
        }
    }
}
