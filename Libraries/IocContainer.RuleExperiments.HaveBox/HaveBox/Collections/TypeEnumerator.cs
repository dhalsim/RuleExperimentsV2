using HaveBox.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HaveBox.Collections
{
    public class TypeEnumerator<T> : IEnumerator<T>
    {
        IEnumerator<TypeDetails> _typeDetailEnumerator;
        IDictionary<Type, object> _instanciatedTypes;

        internal TypeEnumerator(IEnumerator<TypeDetails> typeDetailEnumerator, IDictionary<Type, object> instanciatedTypes)
        {
            _typeDetailEnumerator = typeDetailEnumerator;
            _instanciatedTypes = instanciatedTypes;
        }

        public T Current
        {
            get { return GetCurrent(); }
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get { return GetCurrent(); }
        }

        public bool MoveNext()
        {
            return _typeDetailEnumerator.MoveNext();
        }

        public void Reset()
        {
            _typeDetailEnumerator.Reset();
        }

        private T GetCurrent()
        {
            if (_typeDetailEnumerator.Current == null)
            {
                return default(T);
            }

            object instance;

            if (!_instanciatedTypes.TryGetValue(_typeDetailEnumerator.Current.ImplementType, out instance))
            {
                _typeDetailEnumerator.Current.CreateInstanceDelegate(_typeDetailEnumerator.Current, out instance);
                _instanciatedTypes[_typeDetailEnumerator.Current.ImplementType] = instance;
            }
            return (T) instance;
        }
    }
}
