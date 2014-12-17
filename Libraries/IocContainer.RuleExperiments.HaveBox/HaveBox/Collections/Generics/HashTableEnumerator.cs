using System.Collections;
using System.Collections.Generic;

namespace HaveBox.Collections.Generic
{
    public class HashTableEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        IDictionary<TKey, TValue> _dictionary;
        IEnumerator<TKey> _keys;

        public HashTableEnumerator(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
            _keys = dictionary.Keys.GetEnumerator();
        }

        public KeyValuePair<TKey, TValue> Current
        {
            get 
            {
                return new KeyValuePair<TKey, TValue>(_keys.Current, _dictionary[_keys.Current]);
            }
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get { return (object)Current; }
        }

        public bool MoveNext()
        {
            return _keys.MoveNext();
        }

        public void Reset()
        {
            _keys.Reset();
        }
    }
}
