using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HaveBox.Collections.Generic
{
    public class HashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private class Entry
        {
            public TKey Key;
            public TValue Value;

            public Entry nextEntry;
        }

        private Entry[] _buckets;

        private int _entriesCount;
        private int _mod;
        private decimal _loadfactorThredshold;
        private decimal _loadfactor;
        private int _initialBucketCapacity;

        public HashTable()
            : this(1)
        {

        }

        public HashTable(int bucketCapacity)
            : this(bucketCapacity, 0.75m)
        {
        }

        public HashTable(int bucketCapacity, decimal loadfactor)
        {
            _loadfactorThredshold = loadfactor;
            _initialBucketCapacity = bucketCapacity;
            Initialize(_initialBucketCapacity);
        }

        private void Initialize(int bucketCapacity)
        {
            _entriesCount = 0;
            _buckets = new Entry[bucketCapacity];
            _mod = 0X4FFFFFFF & (_buckets.Length - 1);
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException("Trying to add duplicate");
            }

            AddToHashTable(key, value);
        }

        private void AddToHashTable(TKey key, TValue value)
        {
            var targetBucket = 0X4FFFFFFF & key.GetHashCode() & _mod;

            _entriesCount++;
            _buckets[targetBucket] = new Entry
            {
                Key = key,
                Value = value,
                nextEntry = _buckets[targetBucket],
            };

            _loadfactor = (decimal)_entriesCount / (decimal)_buckets.Length;
            if (_loadfactor > _loadfactorThredshold)
            {
                Resize();
            }
        }

        private void Resize()
        {
            var newbuckets = new Entry[_buckets.Length << 1];

            foreach (var bucket in _buckets)
            {
                AddEntriesToNewBucket(newbuckets, bucket);
            }

            _buckets = newbuckets;
            _mod = 0X4FFFFFFF & (_buckets.Length - 1);
        }

        private void AddEntriesToNewBucket(Entry[] bucket, Entry entry)
        {
            if (entry == null)
            {
                return;
            }

            var keyHash = 0X4FFFFFFF & entry.Key.GetHashCode();
            int bucketsCount = bucket.Length;
            var targetBucket = keyHash & bucketsCount - 1;

            bucket[targetBucket] = new Entry
            {
                Key = entry.Key,
                Value = entry.Value,
                nextEntry = bucket[targetBucket],
            };

            AddEntriesToNewBucket(bucket, entry.nextEntry);
        }

        public bool ContainsKey(TKey key)
        {
            var entry = _buckets[key.GetHashCode() & _mod];
            while (entry != null)
            {
                if (key.Equals(entry.Key))
                {
                    return true;
                }

                entry = entry.nextEntry;
            }

            return false;
        }

        public ICollection<TKey> Keys
        {
            get
            {
                var collection = new Collection<TKey>();

                foreach (var entry in _buckets)
                {
                    var currentEntry = entry;
                    while (currentEntry != null)
                    {
                        collection.Add(currentEntry.Key);
                        currentEntry = currentEntry.nextEntry;
                    }
                }

                return collection;
            }
        }

        public bool Remove(TKey key)
        {
            var targetBucket = key.GetHashCode() & _mod;
            var entry = _buckets[targetBucket];

            if (key.Equals(entry.Key))
            {
                _buckets[targetBucket] = null;
                return true;
            }

            var childEntry = entry.nextEntry;

            while (childEntry != null)
            {
                if (key.Equals(childEntry.Key))
                {
                    entry.nextEntry = childEntry.nextEntry;
                    return true;
                }

                childEntry = childEntry.nextEntry;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var entry = _buckets[key.GetHashCode() & _mod];
            while (entry != null)
            {
                if (key.Equals(entry.Key))
                {
                    value = entry.Value;
                    return true;
                }

                entry = entry.nextEntry;
            }

            value = default(TValue);
            return false;
        }

        public ICollection<TValue> Values
        {
            get
            {
                var collection = new Collection<TValue>();

                foreach (var entry in _buckets)
                {
                    var currentEntry = entry;
                    while (currentEntry != null)
                    {
                        collection.Add(currentEntry.Value);
                        currentEntry = currentEntry.nextEntry;
                    }
                }

                return collection;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                var entry = _buckets[key.GetHashCode() & _mod];
                while (entry != null)
                {
                    if (key.Equals(entry.Key))
                    {
                        return entry.Value;
                    }

                    entry = entry.nextEntry;
                }

                throw new ArgumentOutOfRangeException("Unkown key: " + key.ToString());
            }
            set
            {
                AddToHashTable(key, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _entriesCount = 0;
            Initialize(_initialBucketCapacity);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _entriesCount; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new HashTableEnumerator<TKey, TValue>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
