using SheetSetManager.SheetManager.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Managers
{
    internal class SheetsManager : IList<SheetModel>
    {
        private readonly List<SheetModel> _sheets = new List<SheetModel>();
        
        #region IList implementation
        public SheetModel this[int index]
        {
            get => _sheets[index];
            set => _sheets[index] = value;
        }
        public int Count => _sheets.Count;
        public bool IsReadOnly => false;
        public void Add(SheetModel item)
        {
            _sheets.Add(item);
        }
        public void Clear()
        {
            _sheets.Clear();
        }
        public bool Contains(SheetModel item)
        {
            return _sheets.Contains(item);
        }
        public void CopyTo(SheetModel[] array, int arrayIndex)
        {
            _sheets.CopyTo(array, arrayIndex);
        }
        public IEnumerator<SheetModel> GetEnumerator()
        {
            return _sheets.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public int IndexOf(SheetModel item)
        {
            return _sheets.IndexOf(item);
        }

        public void Insert(int index, SheetModel item)
        {
            _sheets.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _sheets.RemoveAt(index);
        }

        public bool Remove(SheetModel item)
        {
            return _sheets.Remove(item);
        } 
        #endregion
    }
}