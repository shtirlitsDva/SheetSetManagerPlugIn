using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACSMCOMPONENTS25Lib;
using System.Collections;
using System.Reflection;

namespace SheetSetManager.Wrappers
{
    public class AcSmComEnumerator : IEnumerable<IAcSmComponent>, IEnumerator<IAcSmComponent>
    {
        private readonly IAcSmEnumComponent _enumerator;
        private IAcSmComponent _current;

        public AcSmComEnumerator(IAcSmEnumComponent enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }

        public IAcSmComponent Current => _current;
        object IEnumerator.Current => _current;

        public bool MoveNext()
        {
            _current = _enumerator.Next();
            return _current != null;
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        public void Dispose() { }

        public IEnumerator<IAcSmComponent> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}