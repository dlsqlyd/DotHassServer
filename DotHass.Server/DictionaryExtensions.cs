// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace System.Collections.Generic
{
    internal static class DictionaryExtensions
    {
        internal static T Get<T>(this IDictionary<string, object> dictionary, string key)
        {
            return dictionary.TryGetValue(key, out object value) ? (T)value : default;
        }

        internal static T Get<T>(this IDictionary<string, object> dictionary, string subDictionaryKey, string key)
        {
            var subDictionary = dictionary.Get<IDictionary<string, object>>(subDictionaryKey);
            if (subDictionary == null)
            {
                return default;
            }

            return subDictionary.Get<T>(key);
        }
    }
}