using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TwitchPointsAuction.Classes
{
	public struct WritableKeyValuePair<TKey, TValue>
	{
		public WritableKeyValuePair(TKey key, TValue value) { Key = key; Value = value; }
		public readonly TKey Key { get; }
		public TValue Value { get; set; }
		public override string ToString() { return $"{Key},{Value}"; }
	} 

	public class NotifyDictionary<TKey, TValue> :
		ICollection<WritableKeyValuePair<TKey, TValue>>, IList<WritableKeyValuePair<TKey, TValue>>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		readonly IList<WritableKeyValuePair<TKey, TValue>> dictionary;
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		public NotifyDictionary() : this(new List<WritableKeyValuePair<TKey, TValue>>())
		{
		}

		public NotifyDictionary(IEnumerable<WritableKeyValuePair<TKey, TValue>> collection) : this(new List<WritableKeyValuePair<TKey, TValue>>(collection))
		{
		}

		public NotifyDictionary(IList<WritableKeyValuePair<TKey, TValue>> list)
		{
			this.dictionary = list;
		}

		void AddWithNotification(TKey key, TValue value)
		{
			AddWithNotification(new WritableKeyValuePair<TKey, TValue>(key, value));
		}

		void AddWithNotification(WritableKeyValuePair<TKey, TValue> keyvaluepair)
		{
			dictionary.Add(keyvaluepair);
			CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
				keyvaluepair));
			PropertyChanged(this, new PropertyChangedEventArgs("Count"));
		}

		void UpdateWithNotification(TKey key, TValue value)
		{
			UpdateWithNotification(new WritableKeyValuePair<TKey, TValue>(key, value));
		}

		void UpdateWithNotification(WritableKeyValuePair<TKey, TValue> newkeyvaluepair)
		{
			if (this.ContainsKey(newkeyvaluepair.Key))
			{
				var index = IndexOf(newkeyvaluepair.Key);
				var oldvalue = dictionary[index];
				dictionary[index] = newkeyvaluepair;

				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
					newkeyvaluepair,oldvalue, index));
			}
			else
			{
				AddWithNotification(newkeyvaluepair);
			}			
		}

		bool RemoveWithNotification(TKey key)
		{
			return RemoveWithNotification(this.IndexOf(key));
		}

		bool RemoveWithNotification(int index)
		{
			var keyvaluepair = dictionary[index];
			if (dictionary.Remove(keyvaluepair))
			{
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
					new WritableKeyValuePair<TKey, TValue>(keyvaluepair.Key, keyvaluepair.Value)));
				PropertyChanged(this, new PropertyChangedEventArgs("Count"));
				return true;
			}

			return false;
		}

		public TValue this[TKey key]
		{
			get { return this[IndexOf(key)].Value; }
			set { UpdateWithNotification(key, value); }
		}

		public WritableKeyValuePair<TKey, TValue> this[int index] 
		{
			get { return index >= 0 && index < this.Count() ? dictionary[index] : default(WritableKeyValuePair<TKey, TValue>); }
			set { UpdateWithNotification(value); }
		}

		public void Add(TKey key, TValue value)
		{
			AddWithNotification(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			return dictionary.Any(x=>x.Key.Equals(key));
		}

		public ICollection<TKey> Keys
		{
			get { return (ICollection<TKey>)dictionary.Select(x=>x.Key); }
		}

		public ICollection<TValue> Values
		{
			get { return (ICollection<TValue>)dictionary.Select(x => x.Value); }
		}

		public bool Remove(TKey key)
		{
			return RemoveWithNotification(key);
		}

		public void RemoveAt(int index)
		{
			RemoveWithNotification(index);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			throw new NotImplementedException();
		}


		void ICollection<WritableKeyValuePair<TKey, TValue>>.Add(WritableKeyValuePair<TKey, TValue> item)
		{
			AddWithNotification(item);
		}

		void ICollection<WritableKeyValuePair<TKey, TValue>>.Clear()
		{
			((ICollection<WritableKeyValuePair<TKey, TValue>>)dictionary).Clear();

			CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			PropertyChanged(this, new PropertyChangedEventArgs("Count"));
		}

		bool ICollection<WritableKeyValuePair<TKey, TValue>>.Contains(WritableKeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<WritableKeyValuePair<TKey, TValue>>)dictionary).Contains(item);
		}

		void ICollection<WritableKeyValuePair<TKey, TValue>>.CopyTo(WritableKeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<WritableKeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
		}

		int ICollection<WritableKeyValuePair<TKey, TValue>>.Count
		{
			get { return ((ICollection<WritableKeyValuePair<TKey, TValue>>)dictionary).Count; }
		}

		bool ICollection<WritableKeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get { return ((ICollection<WritableKeyValuePair<TKey, TValue>>)dictionary).IsReadOnly; }
		}

		bool ICollection<WritableKeyValuePair<TKey, TValue>>.Remove(WritableKeyValuePair<TKey, TValue> item)
		{
			return RemoveWithNotification(item.Key);
		}


		IEnumerator<WritableKeyValuePair<TKey, TValue>> IEnumerable<WritableKeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return ((ICollection<WritableKeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((ICollection<WritableKeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();
		}

		public int IndexOf(TKey key)
		{
			return dictionary.IndexOf(dictionary.FirstOrDefault(x => x.Key.Equals(key)));
		}

		public int IndexOf(WritableKeyValuePair<TKey, TValue> item)
		{
			return dictionary.IndexOf(item);
		}

		public void Insert(int index, WritableKeyValuePair<TKey, TValue> item)
		{
			throw new NotImplementedException();
		}
	}
}
