using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotHass.Repository.Entity
{
    using KeysDict = IDictionary<Type, List<PropertyInfo>>;

    public static class EntityKeyInfo
    {
        internal static readonly KeysDict GroupKeyMapping = new ConcurrentDictionary<Type, List<PropertyInfo>>();
        internal static readonly KeysDict PrimaryKeyMapping = new ConcurrentDictionary<Type, List<PropertyInfo>>();

        internal const char KeyCodeJoinChar = '-';

        /// 生成Key
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string CreateKeyCode(params object[] keys)
        {
            string value = "";
            foreach (var key in keys)
            {
                if (value.Length > 0)
                {
                    value += KeyCodeJoinChar;
                }
                value += EncodeKeyCode(key.ToString());
            }
            return value;
        }

        /// <summary>
        /// Conver '-' char
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static string EncodeKeyCode(string keyCode)
        {
            return (keyCode ?? "")
                .Replace(KeyCodeJoinChar.ToString(), "%45")
                .Replace("_", "%46")
                .Replace("|", "%7C");
        }

        /// <summary>
        // checks for properties in this order that match TKey type
        //  1) RepositoryGroupKeyAttribute
        //  2) Id
        //  3) [Type Name]Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetGroupKey<T>(T t)
        {
            return GetKeyCode<T, RepositoryGroupKeyAttribute>(t, GroupKeyMapping);
        }

        public static List<PropertyInfo> GetGroupProps<T>()
        {
            return GetKeyPropertyInfo<T, RepositoryGroupKeyAttribute>(GroupKeyMapping);
        }

        /// <summary>
        /// 实体中唯一的key
        ///
        //  1) RepositoryPrimaryKeyAttribute
        //  2) Id
        //  3) [Type Name]Id
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetPrimaryKey<T>(T t)
        {
            return GetKeyCode<T, RepositoryPrimaryKeyAttribute>(t, PrimaryKeyMapping);
        }

        public static List<PropertyInfo> GetPrimaryProps<T>()
        {
            return GetKeyPropertyInfo<T, RepositoryPrimaryKeyAttribute>(PrimaryKeyMapping);
        }

        private static string GetKeyCode<T, TAttribute>(T t, KeysDict datasource) where TAttribute : Attribute
        {
            string key = String.Empty;

            var Keys = GetKeyPropertyInfo<T, TAttribute>(datasource);

            foreach (PropertyInfo propInfo in Keys)
            {
                if (key.Length > 0)
                {
                    key += KeyCodeJoinChar;
                }
                key += EncodeKeyCode(propInfo.GetValue(t, null).ToString());
            }

            return key;
        }

        private static List<PropertyInfo> GetKeyPropertyInfo<TEntity, TAttribute>(KeysDict datasource) where TAttribute : Attribute
        {
            var type = typeof(TEntity);
            if (datasource.ContainsKey(type))
            {
                return datasource[type];
            }
            var props = new List<PropertyInfo>();

            var propertys = GetKeyPropertys<TAttribute>(type);
            if (propertys.Count == 0) return null;

            //查找属性是不是在父类
            foreach (var item in propertys)
            {
                var propertyName = item.Name;

                var propInfo = type.GetTypeInfo().GetDeclaredProperty(propertyName);

                //如果类型有父类。且是定义在父类
                while (propInfo == null && type.GetTypeInfo().BaseType != null)
                {
                    type = type.GetTypeInfo().BaseType;
                    propInfo = type.GetTypeInfo().GetDeclaredProperty(propertyName);
                }

                if (propInfo != null)
                {
                    props.Add(propInfo);
                }
            }
            datasource.Add(type, props);
            return props;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private static List<PropertyInfo> GetKeyPropertys<TAttribute>(Type entityType) where TAttribute : Attribute
        {
            var result = new List<PropertyInfo>();

            //GetRuntimeProperties此方法返回指定类型，包括继承、 非公共实例和静态属性上定义的所有属性。所以会包括父类的
            //获取拥有TAttribute的属性
            result = entityType.GetRuntimeProperties().Where(x => x.HasAttribute<TAttribute>()).ToList();
            if (result.Count > 0) return result;

            //Id和[Type Name]
            foreach (var propertyName in GetKeyNameChecks(entityType))
            {
                var propInfo = GetPropertyCaseInsensitive(entityType, propertyName);

                if (propInfo != null)
                {
                    result.Add(propInfo);
                    //直接break。Id和[Type Name]Id只允许出现一个
                    break;
                }
            }
            return result;
        }

        public static string KeySuffix = "Id";

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetKeyNameChecks(Type type)
        {
            var suffix = KeySuffix;
            var typeName = type.Name;
            return new[] { suffix, typeName + suffix };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static PropertyInfo GetPropertyCaseInsensitive(Type type, string propertyName)
        {
            return type.GetRuntimeProperties().Where(pi => pi.Name.ToLowerInvariant() == propertyName.ToLowerInvariant()).FirstOrDefault();
        }
    }
}