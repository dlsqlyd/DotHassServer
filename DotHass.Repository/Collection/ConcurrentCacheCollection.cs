using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DotHass.Repository.Collection
{
    public class ConcurrentCacheCollection : IDataCollection
    {
        private ConcurrentDictionary<string, object> _cacheStruct;

        /// <summary>
        ///
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="disableEvent">是否禁用事件</param>
        public ConcurrentCacheCollection(int capacity = 0)
        {
            _cacheStruct = capacity > 0
                ? new ConcurrentDictionary<string, object>(Environment.ProcessorCount, capacity)
                : new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out object value)
        {
            return _cacheStruct.TryGetValue(key, out value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                return _cacheStruct[key];
            }
            set
            {
                _cacheStruct[key] = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add<T>(string key, T value)
        {
            TryAdd(key, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="updateValueFactory"></param>
        /// <returns></returns>
        public T AddOrUpdate<T>(string key, T data, Func<string, T, T> updateValueFactory)
        {
            //CacheItemChangeType changeType = CacheItemChangeType.Add;
            object func(string updateKey, object updateValue)
            {
                //changeType = CacheItemChangeType.Modify;
                return updateValueFactory(updateKey, (T)updateValue);
            }
            T temp = (T)_cacheStruct.AddOrUpdate(key, data, func);
            //AddChildrenListener(temp);
            //Notify(temp, changeType, PropertyName);
            return temp;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _cacheStruct.ContainsKey(key);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            TryAdd(key, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return TryRemove(key, out object data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryAdd<T>(string key, T data)
        {
            if (_cacheStruct.TryAdd(key, data))
            {
                //AddChildrenListener(data);
                //Notify(data, CacheItemChangeType.Add, PropertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public T GetOrAdd<T>(string key, Func<string, T> valueFactory)
        {
            return (T)_cacheStruct.GetOrAdd(key, updateKey => valueFactory(updateKey));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryGetValue<T>(string key, out T data)
        {
            data = default;
            if (_cacheStruct.TryGetValue(key, out object temp))
            {
                data = (T)temp;
                return true;
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="newData"></param>
        /// <param name="compData"></param>
        /// <returns></returns>
        public bool TryUpdate<T>(string key, T newData, T compData)
        {
            if (_cacheStruct.TryUpdate(key, newData, compData))
            {
                //AddChildrenListener(newData);
                //Notify(newData, CacheItemChangeType.Modify, PropertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryRemove<T>(string key, out T data)
        {
            data = default;
            if (_cacheStruct.TryRemove(key, out object temp))
            {
                data = (T)temp;
                //Notify(data, CacheItemChangeType.Remove, PropertyName);
                //RemoveChildrenListener(data);
                return true;
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        ///
        /// </summary>
        public void Clear()
        {
            _cacheStruct.Clear();
            //ClearChildrenEvent();
            //Notify(this, CacheItemChangeType.Clear, PropertyName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return ContainsKey(item.Key);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            int index = 0;
            var enumerator = _cacheStruct.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (index >= arrayIndex && index < array.Length)
                {
                    array[index] = enumerator.Current;
                }
                index++;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, object> item)
        {
            return TryRemove(item.Key, out object data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<KeyValuePair<string, T>> ToList<T>()
        {
            var list = new List<KeyValuePair<string, T>>();
            var enumerator = _cacheStruct.GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add(new KeyValuePair<string, T>(enumerator.Current.Key, (T)enumerator.Current.Value));
            }
            return list;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void Foreach<T>(Func<string, T, bool> func)
        {
            var enumerator = _cacheStruct.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!func(enumerator.Current.Key, (T)enumerator.Current.Value))
                {
                    break;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="groupKey"></param>
        public void Foreach<T>(Func<string, string, T, bool> func, string groupKey)
        {
            var enumerator = _cacheStruct.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!func(groupKey, enumerator.Current.Key, (T)enumerator.Current.Value))
                {
                    break;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, object>> GetEnumerable()
        {
            return _cacheStruct;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _cacheStruct.GetEnumerator();
        }

        /// <summary>
        ///
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return _cacheStruct.Keys;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ICollection<object> Values
        {
            get
            {
                return _cacheStruct.Values;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public int Count
        {
            get { return _cacheStruct.Count; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsEmpty
        {
            get { return _cacheStruct.IsEmpty; }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Flag: Has Dispose already been called?
        private bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                _cacheStruct = null;
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~ConcurrentCacheCollection()
        {
            Dispose(false);
        }
    }
}