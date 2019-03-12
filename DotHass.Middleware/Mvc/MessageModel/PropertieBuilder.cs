using DotHass.Middleware.Mvc.Controller;
using Microsoft.Extensions.Primitives;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotHass.Middleware.Mvc.MessageModel
{
    public class PropertieBuilder
    {
        #region 建立属性

        public static IDictionary<string, object> Build(MethodInfo method, OwinContext context, out bool error)
        {
            error = false;

            ParameterInfo[] parameterInfos = method.GetParameters();

            var fromdata = new FormReader(context.Request.Body).ReadForm();
            var parameters = new Dictionary<string, object>();
            if (parameterInfos.Length == 1)
            {
                var parameterInfo = parameterInfos.FirstOrDefault();

                var paramType = parameterInfo.ParameterType;
                if (paramType.IsClass == true &&
                    paramType.IsValueType == false &&
                    paramType != typeof(string) &&
                    paramType != typeof(StringValues) &&
                    paramType != typeof(Dictionary<string, StringValues>))
                {
                    object msgModel = null;
                    try
                    {
                        msgModel = BindParamModel(paramType, fromdata);
                    }
                    catch (Exception)
                    {
                        context.Response.StatusCode = ControllerStatusCodes.Status1101ControllerParamBuildFail;
                        error = true;
                    }
                    if (msgModel != null)
                    {
                        parameters[parameterInfo.Name] = msgModel;
                    }
                    return parameters;
                }
            }

            try
            {
                for (var index = 0; index < parameterInfos.Length; index++)
                {
                    var parameterInfo = parameterInfos[index];
                    //如果是最后一个参数,且是一个字典
                    if (index == parameterInfos.Length - 1 && parameterInfo.ParameterType == typeof(Dictionary<string, StringValues>))
                    {
                        parameters.Add(parameterInfo.Name, fromdata);
                        continue;
                    }
                    //如果form中不存在则设置默认值
                    if (fromdata.TryGetValue(parameterInfo.Name, out StringValues value) == false)
                    {
                        parameters.Add(parameterInfo.Name, GetDefaultValue(parameterInfo.ParameterType));
                        continue;
                    }
                    //如果form中有该参数的名字的
                    if (value.Count == 1)
                    {
                        parameters.Add(parameterInfo.Name, GetParamStringValue(value.ToString(), parameterInfo.ParameterType));
                    }
                    else if (value.Count > 1)
                    {
                        //如果form中的stringvalues有多个值则转化为数组
                        parameters.Add(parameterInfo.Name, value.ToArray());
                    }
                    //移除已经赋值的key
                    fromdata.Remove(parameterInfo.Name);
                }
            }
            catch (Exception)
            {
                context.Response.StatusCode = ControllerStatusCodes.Status1101ControllerParamBuildFail;
                error = true;
            }
            return parameters;
        }

        public static object GetParamStringValue(string data, Type type)
        {
            try
            {
                if (type.IsEnum)
                {
                    return Enum.Parse(type, data.ToString(), true);
                }

                return Convert.ChangeType(data, type);
            }
            catch (InvalidCastException)
            {
                return GetDefaultValue(type);
            }
        }

        public static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }

        private static object BindParamModel(Type parameterType, Dictionary<string, StringValues> fromdata)
        {
            PropertyInfo[] props = parameterType.GetProperties();
            object res = Activator.CreateInstance(parameterType);
            for (int i = 0; i < props.Length; i++)
            {
                var propInfo = props[i];
                if (propInfo.CanWrite == false)
                {
                    continue;
                }
                //如果form中不存在
                if (fromdata.TryGetValue(propInfo.Name, out StringValues value) == false)
                {
                    continue;
                }
                //如果form中有该参数的名字的
                if (value.Count == 1)
                {
                    propInfo.SetValue(res, GetParamStringValue(value.ToString(), propInfo.PropertyType), null);
                }
                else if (value.Count > 1)
                {  //如果form中的stringvalues有多个值则转化为数组
                    propInfo.SetValue(res, value.ToArray(), null);
                }
            }
            return res;
        }

        #endregion 建立属性
    }
}