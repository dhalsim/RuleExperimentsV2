using HaveBox.Collections.Generic;
using HaveBox.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HaveBox.Collections
{
    public class TypeEnumerable<T> : IEnumerable<T>, ITypeEnumerable
    {
        private IEnumerable<TypeDetails> _typeDetails;
        private IDictionary<Type, object> _instanciatedTypes;

        public TypeEnumerable(IEnumerable<TypeDetails> typeDetails)
        {
            _typeDetails = typeDetails;
        }

        public IEnumerator<T> GetEnumerator()
        {
            _instanciatedTypes = _instanciatedTypes ?? new HashTable<Type, object>();
            return new TypeEnumerator<T>(_typeDetails.GetEnumerator(), _instanciatedTypes);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
