using System;
using System.Collections.Generic;

namespace DotHass.Repository.Collection
{
    public interface IDataCollection : IDictionary<string, object>, IDisposable
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Add<T>(string key, T value);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool TryAdd<T>(string key, T data);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        T GetOrAdd<T>(string key, Func<string, T> valueFactory);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool TryGetValue<T>(string key, out T data);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="newData"></param>
        /// <param name="compData"></param>
        /// <returns></returns>
        bool TryUpdate<T>(string key, T newData, T compData);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool TryRemove<T>(string key, out T data);

        /// <summary>
        /// 增加或更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="updateValueFactory"></param>
        /// <returns></returns>
        T AddOrUpdate<T>(string key, T data, Func<string, T, T> updateValueFactory);
    }
}