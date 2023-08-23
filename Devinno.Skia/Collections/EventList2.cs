using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Collections
{
    public class AppendEventArgs<T> : EventArgs
    {
        public List<T> Items { get; } = new List<T>();
        public AppendEventArgs(IEnumerable<T> list) => Items.AddRange(list);
    }

    public class EventList2<T> : List<T>
    {
        public event EventHandler Changed;
        public event EventHandler<AppendEventArgs<T>> Appended;

        #region Append
        public new void Add(T item)
        {
            base.Add(item);
            InvokeChanged();
            InvokeAppended(new T[] { item });
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            InvokeChanged();
            InvokeAppended(collection);
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            InvokeChanged();
            InvokeAppended(new T[] { item });
        }

        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            InvokeChanged();
            InvokeAppended(collection);
        }
        #endregion
        #region Clear
        public new void Clear()
        {
            base.Clear();
            InvokeChanged();
        }
        #endregion
        #region Remove
        public new bool Remove(T item)
        {
            var ret = base.Remove(item);
            InvokeChanged(); 
            return ret;
        }
        public new int RemoveAll(Predicate<T> match)
        {
            var ret = base.RemoveAll(match);
            InvokeChanged(); 
            return ret;
        }
        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            InvokeChanged();
        }
        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            InvokeChanged();
        }
        #endregion
        #region Reverse
        public new void Reverse(int index, int count)
        {
            base.Reverse(index, count);
            InvokeChanged();
        }

        public new void Reverse()
        {
            base.Reverse();
            InvokeChanged();
        }
        #endregion
        #region Sort
        public new void Sort(int index, int count, IComparer<T> comparer)
        {
            base.Sort(index, count, comparer);
            InvokeChanged();
        }

        public new void Sort(Comparison<T> comparison)
        {
            base.Sort(comparison);
            InvokeChanged();
        }

        public new void Sort()
        {
            base.Sort();
            InvokeChanged();
        }

        public new void Sort(IComparer<T> comparer)
        {
            base.Sort(comparer);
            InvokeChanged();
        }
        #endregion

        #region Override
        protected virtual void OnChanged() { }
        protected virtual void OnAppeded(IEnumerable<T> items) { }
        #endregion

        #region Invoke
        private void InvokeAppended(IEnumerable<T> items)
        {
            Appended?.Invoke(this, new AppendEventArgs<T>(items));
            OnAppeded(items);
        }

        private void InvokeChanged()
        {
            Changed?.Invoke(this, new EventArgs());
            OnChanged();
        }
        #endregion
    }
}
