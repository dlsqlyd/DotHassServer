using System;
using System.Collections;
using System.Collections.Generic;

namespace DotHass.Repository.Collection
{
    public class ReadonlyCacheCollection : IDataCollection
    {
        //只读使用的是dict。。读取速度。。提高
        private Dictionary<string, object> _cacheStruct;

        /// <summary>
        ///
        /// </summary>
        public ReadonlyCacheCollection()
        {
            _cacheStruct = new Dictionary<string, object>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out object value)
        {
            return TryGetValue(key, out value);
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
        /// <param name="data"></param>
        /// <param name="updateValueFactory"></param>
        /// <returns></returns>
        public T AddOrUpdate<T>(string key, T data, Func<string, T, T> updateValueFactory)
        {
            T updateData = updateValueFactory(key, data);
            if (!_cacheStruct.ContainsKey(key))
            {
                _cacheStruct.Add(key, updateData);
            }
            else
            {
                _cacheStruct[key] = updateData;
            }
            return updateData;
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
        /// <returns></returns>
        public bool TryAdd<T>(string key, T data)
        {
            if (!_cacheStruct.ContainsKey(key))
            {
                _cacheStruct.Add(key, data);
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
            T newValue = valueFactory(key);
            if (!_cacheStruct.ContainsKey(key))
            {
                _cacheStruct.Add(key, newValue);
                return newValue;
            }
            return (T)_cacheStruct[key];
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
            if (_cacheStruct.ContainsKey(key))
            {
                data = (T)_cacheStruct[key];
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
            if (_cacheStruct.ContainsKey(key))
            {
                T oldValue = (T)_cacheStruct[key];
                _cacheStruct[key] = newData;
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
            if (_cacheStruct.ContainsKey(key))
            {
                data = (T)_cacheStruct[key];
                _cacheStruct.Remove(key);
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
            return _cacheStruct.Remove(item.Key);
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
            get { return _cacheStruct.Keys; }
        }

        /// <summary>
        ///
        /// </summary>
        public ICollection<object> Values
        {
            get { return _cacheStruct.Values; }
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
            get { return _cacheStruct.Count == 0; }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

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

        ~ReadonlyCacheCollection()
        {
            Dispose(false);
        }
    }
}