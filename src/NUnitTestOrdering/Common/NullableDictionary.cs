// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : NullableDictionary.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************

namespace NUnitTestOrdering.Common {
    using System.Collections;
    using System.Collections.Generic;

    internal sealed class NullableDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        private readonly IDictionary<TKey, TValue> _innerStore;

        public NullableDictionary() {
            this._innerStore = new Dictionary<TKey, TValue>();
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            this._innerStore.Add(item);
        }

        public void Add(TKey key, TValue value) {
            this._innerStore.Add(key, value);
        }

        public void Clear() {
            this._innerStore.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return this._innerStore.Contains(item);
        }

        public bool ContainsKey(TKey key) {
            return this._innerStore.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            this._innerStore.CopyTo(array, arrayIndex);
        }

        public int Count => this._innerStore.Count;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return this._innerStore.GetEnumerator();
        }

        public bool IsReadOnly => this._innerStore.IsReadOnly;

        public TValue this[TKey key] {
            get {
                TValue val;
                this._innerStore.TryGetValue(key, out val);
                return val;
            }
            set { this._innerStore[key] = value; }
        }

        public ICollection<TKey> Keys => this._innerStore.Keys;

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            return this._innerStore.Remove(item);
        }

        public bool Remove(TKey key) {
            return this._innerStore.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return this._innerStore.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values => this._innerStore.Values;
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) this._innerStore).GetEnumerator();
        }
    }
}
