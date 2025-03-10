using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using ACSMCOMPONENTS25Lib;
using System.Collections;
using System.Reflection;

namespace ABF_SheetSetManager.Wrappers
{
    internal class AcSmPropertyEnumerator : IEnumerable<AcSmProperty>, IEnumerator<AcSmProperty>
    {
        private readonly IAcSmEnumProperty _enumerator;
        private AcSmProperty _current;

        public AcSmPropertyEnumerator(IAcSmEnumProperty enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }

        public AcSmProperty Current => _current;
        object IEnumerator.Current => _current;

        public bool MoveNext()
        {
            string name = null;
            AcSmCustomPropertyValue value = null;
            _enumerator.Next(out name, out value);
            
            if (!string.IsNullOrEmpty(name))
            {
                _current = new AcSmProperty(name, value);
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        public void Dispose() { }

        public IEnumerator<AcSmProperty> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}