using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TwitchPointsAuction.Classes
{
    public class NotifyCollection<T> : ICollection<T>, IEnumerable<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private List<T> _Collection;
        private const string CountString = "Count";
        public NotifyCollection()
        {
            _Collection = new List<T>();
        }

        public T this[int i]
        {
            get
            {
                return i >= 0 && i < Count ? _Collection[i] : default(T);
            }
            set
            {
                _Collection[i] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value));
            }
        }

        public void Add(T item)
        {
            _Collection.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public bool Remove(T item)
        {
            var index = _Collection.IndexOf(item);
            _Collection.Remove(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            return true;
        }

        public void Clear()
        {
            if (Count > 0)
            {
                _Collection.Clear();
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }


        public bool Contains(T item)
        {
            return _Collection.Contains(item);
        }

        public void CopyTo(T[] array, int index)
        {
            _Collection.AddRange(array);
        }

        public int Count
        {
            get { return _Collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(CountString);
            CollectionChanged?.Invoke(this, e);
        }
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T val in _Collection)
            {
                yield return val;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
