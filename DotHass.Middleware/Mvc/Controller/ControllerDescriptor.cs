using DotHass.Middleware.Mvc.Routing;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DotHass.Middleware.Mvc.Controller
{
    public class ControllerDescriptor
    {
        private ControllerOptions options;

        public ControllerDescriptor(TypeInfo item, ControllerOptions options)
        {
            Id = Guid.NewGuid().ToString();
            this.options = options;
            this.ControllerType = item;
            this.HandlerBeforeMethodInfo = FindMethod(ControllerType, options.HandlerBeforeMethodName, typeof(bool), false);
            this.HandlerMethodInfo = FindMethod(ControllerType, options.HandlerMethodName, options.HandlerRetureType);
            this.HandlerAfterMethodInfo = FindMethod(ControllerType, options.HandlerAfterMethodName, null, false);
            this.InitContractId();
        }

        /// <summary>
        /// Gets an id which uniquely identifies the action.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets or sets the collection of route values that must be provided by routing
        /// for the action to be selected.
        /// </summary>
        public int ContractID { get; set; }

        public TypeInfo ControllerType { get; set; }

        public MethodInfo HandlerBeforeMethodInfo { get; set; }

        public MethodInfo HandlerMethodInfo { get; set; }

        public MethodInfo HandlerAfterMethodInfo { get; set; }

        /// <summary>
        /// 1.类似 _1000_UserController
        /// 2.查找RouteAttribute
        /// 3.使用常量属性ContractIdName
        /// </summary>
        public void InitContractId()
        {
            var nameArr = ControllerType.Name.Split("_", StringSplitOptions.RemoveEmptyEntries);

            if (nameArr.Length == 2)
            {
                this.ContractID = Convert.ToInt32(nameArr[0]);
                return;
            }

            var contract = ControllerType.GetCustomAttribute<RouteAttribute>();
            if (contract != null)
            {
                this.ContractID = Int32.Parse(contract.Template.Split("/", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
                return;
            }

            var field = ControllerType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name == options.ContractIdName).FirstOrDefault();
            if (field != null)
            {
                this.ContractID = Convert.ToInt32(field.GetRawConstantValue());
                return;
            }
        }

        public MethodInfo FindMethod(Type controllerType, string methodName, Type returnType = null, bool required = true)
        {
            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var selectedMethods = methods.Where(method => method.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (selectedMethods.Count > 1)
            {
                throw new InvalidOperationException(string.Format("Having multiple overloads of method '{0}' is not supported.", methodName));
            }

            var methodInfo = selectedMethods.FirstOrDefault();
            if (methodInfo == null)
            {
                if (required)
                {
                    throw new InvalidOperationException(string.Format("A public method named '{0}'  could not be found in the '{1}' type.",
                        methodName,
                        controllerType.FullName));
                }
                return null;
            }

            if (returnType != null)
            {
                var rType = methodInfo.ReturnType;
                var typeIsRight = false;
                if (rType == returnType)
                {
                    typeIsRight = true;
                }

                if (rType.GetTypeInfo().IsGenericType &&
                    rType.GetGenericTypeDefinition() == typeof(Task<>) &&
                    rType.GenericTypeArguments[0] == returnType)
                {
                    typeIsRight = true;
                }

                if (typeIsRight == false)
                {
                    if (required)
                    {
                        throw new InvalidOperationException(string.Format("The '{0}' method in the type '{1}' must have a return type of '{2}'.",
                            methodInfo.Name,
                            controllerType.FullName,
                            returnType.Name));
                    }
                    return null;
                }
            }
            return methodInfo;
        }
    }
}