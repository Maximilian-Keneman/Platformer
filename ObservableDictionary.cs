using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace System.Collections.ObjectModel
{
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, ISerializable, IDeserializationCallback
    {
        public event EventHandler<ObserveKeyChangedEventArgs<TKey, TValue>> KeyCreated;
        public event EventHandler<ObserveKeyChangingEventArgs<TKey, TValue>> KeyChanging;
        public event EventHandler<ObserveKeyChangedEventArgs<TKey, TValue>> KeyChanged;
        public event EventHandler<ObserveKeyChangedEventArgs<TKey, TValue>> KeyRemoved;

        private Dictionary<TKey, TValue> dictionary;
        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                if (ContainsKey(key))
                {
                    var args = new ObserveKeyChangingEventArgs<TKey, TValue>(key, dictionary[key], value);
                    KeyChanging?.Invoke(this, args);
                    dictionary[key] = args.NewValue;
                    KeyChanged?.Invoke(this, args);
                }
                else
                {
                    throw new KeyNotFoundException($"Ключ \"{key}\" не найден");
                }
            }
        }

        public ObservableDictionary() : this(0, null) { }
        public ObservableDictionary(int capacity) : this(capacity, null) { }
        public ObservableDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer) { }
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null) { }
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) :
            this(dictionary != null ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        public int Count => dictionary.Count;
        public Dictionary<TKey, TValue>.KeyCollection Keys => dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => dictionary.Values;


        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException($"Ключ \"{key}\" уже существует", nameof(key));
            else
            {
                KeyCreated?.Invoke(this, (key, default(TValue), value));
                dictionary.Add(key, value);
            }
        }
        public bool Remove(TKey key)
        {
            if (ContainsKey(key))
            {
                KeyRemoved?.Invoke(this, (key, dictionary[key], default(TValue)));
                dictionary.Remove(key);
                return true;
            }
            else
                return false;
        }
        public void Clear()
        {
            foreach (var pair in dictionary)
                KeyRemoved?.Invoke(this, (pair.Key, pair.Value, default(TValue)));
            dictionary.Clear();
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => dictionary.GetEnumerator();

        #region NotImplemented
        ICollection IDictionary.Keys => Keys;
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        ICollection IDictionary.Values => Values;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        object IDictionary.this[object key] { get => this[(TKey)key]; set => this[(TKey)key] = (TValue)value; }
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
        bool IDictionary.Contains(object key) => ContainsKey((TKey)key);
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        void IDictionary.Add(object key, object value) => Add((TKey)key, (TValue)value);
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
        void IDictionary.Remove(object key) => Remove((TKey)key);

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IDictionaryEnumerator IDictionary.GetEnumerator() => GetEnumerator();

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
        bool IDictionary.IsReadOnly => false;
        bool IDictionary.IsFixedSize => false;

        object ICollection.SyncRoot => throw new NotImplementedException();
        bool ICollection.IsSynchronized => throw new NotImplementedException();
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();
        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Dictionary", dictionary);
        }
        private ObservableDictionary(SerializationInfo info, StreamingContext context)
        {
            dictionary = (Dictionary<TKey, TValue>)info.GetValue("Dictionary", typeof(Dictionary<TKey, TValue>));
        }
        public void OnDeserialization(object sender)
        {
            dictionary.OnDeserialization(sender);
        }
    }

    public class ObserveKeyChangingEventArgs<TKey, TValue> : EventArgs
    {
        public TKey Key { get; }
        public TValue OldValue { get; }
        public TValue NewValue { get; set; }

        public ObserveKeyChangingEventArgs(TKey key, TValue oldValue, TValue newValue)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public static implicit operator ObserveKeyChangingEventArgs<TKey, TValue>((TKey key, TValue oldValue, TValue newValue) tuple)
            => new ObserveKeyChangingEventArgs<TKey, TValue>(tuple.key, tuple.oldValue, tuple.newValue);
        public static implicit operator ObserveKeyChangingEventArgs<TKey, TValue>(ObserveKeyChangedEventArgs<TKey,TValue> args)
            => new ObserveKeyChangingEventArgs<TKey, TValue>(args.Key, args.OldValue, args.NewValue);
    }
    public class ObserveKeyChangedEventArgs<TKey, TValue> : EventArgs
    {
        public TKey Key { get; }
        public TValue OldValue { get; }
        public TValue NewValue { get; }

        public ObserveKeyChangedEventArgs(TKey key, TValue oldValue, TValue newValue)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public static implicit operator ObserveKeyChangedEventArgs<TKey, TValue>((TKey key, TValue oldValue, TValue newValue) tuple)
            => new ObserveKeyChangedEventArgs<TKey, TValue>(tuple.key, tuple.oldValue, tuple.newValue);
        public static implicit operator ObserveKeyChangedEventArgs<TKey, TValue>(ObserveKeyChangingEventArgs<TKey, TValue> args)
            => new ObserveKeyChangedEventArgs<TKey, TValue>(args.Key, args.OldValue, args.NewValue);
    }

    public static class Expansion
    {
        public static ObservableDictionary<TKey, TValue> ToObservableDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
            => new ObservableDictionary<TKey, TValue>(dictionary);
    }
}