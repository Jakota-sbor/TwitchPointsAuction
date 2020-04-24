using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace TwitchPointsAuction.Classes
{
    [JsonObject]
    public class NotifyCollection<T> : ICollection<T>, IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        [JsonProperty]
        private IList<T> Collection;
        [JsonIgnore]
        private const string CountString = "Count";

        public NotifyCollection() : this(new List<T>()) { }

        public NotifyCollection(IList<T> list) 
        {
            Collection = list;
        }

        public T this[int i]
        {
            get
            {
                return i >= 0 && i < Count ? Collection[i] : default(T);
            }
            set
            {
                Collection[i] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value));
            }
        }

        private RelayCommand addremoveCommand;
        [JsonIgnore]
        public RelayCommand AddRemoveCommand
        {
            get
            {
                return addremoveCommand ??
                    (addremoveCommand = new RelayCommand(param =>
                    {
                        var item = (T)param;
                        if (this.Contains(item))
                            this.Remove(item);
                        else
                            this.Add(item);
                    }));
            }
        }

        public void Add(T item)
        {
            if (!Collection.Contains(item))
            {
                Collection.Add(item);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        public bool Remove(T item)
        {
            var index = Collection.IndexOf(item);
            if (index != -1)
            {
                Collection.Remove(item);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                return true;
            }
            return false;
        }

        public void Clear()
        {
            if (Count > 0)
            {
                Collection.Clear();
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public bool Contains(T item)
        {
            return Collection.Contains(item);
        }

        public void CopyTo(T[] array, int index)
        {
            Collection.CopyTo(array,index);
        }

        public int Count
        {
            get { return Collection.Count; }
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
            foreach (T val in Collection)
            {
                yield return val;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return Collection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Collection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Collection.RemoveAt(index);
        }

        public override string ToString()
        {
            var fullstring = string.Empty;
            if (typeof(T).IsEnum)
            {
                var items = Collection.Select(x => (x as Enum).GetDescription()).ToList();

                for (int i = 0; i < items.Count; i++)
                {
                    fullstring += items[i];
                    if (i != items.Count - 1)
                        fullstring += ", ";
                }
            }
            else
            {
                for (int i = 0; i < Collection.Count; i++)
                {
                    fullstring += Collection[i].ToString();
                    if (i != Collection.Count - 1)
                        fullstring += ", ";
                }
            }
            return fullstring;
        }
    }
}
