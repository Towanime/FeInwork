using System.Collections.Generic;
using System.Collections;

namespace FeInwork.Core.Util
{
    public class XboxHashSet<T>
    {
        private Dictionary<T, bool> _dict;

        public XboxHashSet(int capacity)
        {
            _dict = new Dictionary<T, bool>(capacity);
        }

        public XboxHashSet()
        {
            _dict = new Dictionary<T, bool>();
        }

        // Methods

        #region ICollection<T> Members

        public void Add(T item)
        {
            _dict[item] = true;
        }

        /*public void AddWithCheck(T item)
        {
            if (_dict.ContainsKey(item)) return;
            _dict.Add(item, true);
        }*/

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Contains(T item)
        {
            return _dict.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (T item in _dict.Keys)
            {
                array[arrayIndex++] = item;
            }
        }

        public bool Remove(T item)
        {
            return _dict.Remove(item);
        }

        public void MergeInto(XboxHashSet<T> set)
        {
            Dictionary<T, bool>.KeyCollection setKeys = set._dict.Keys;
            foreach (T item in setKeys)
            {
                this._dict[item] = true;
            }
        }

        public Dictionary<T, bool>.KeyCollection.Enumerator GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        // Properties
        public int Count
        {
            get { return _dict.Keys.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}