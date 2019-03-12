using DotHass.Middleware.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotHass.Middleware.Mvc.Controller
{
    public class ControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public const string ControllerTypeNameSuffix = "Controller";

        /// <inheritdoc />
        public void PopulateFeature(
            IEnumerable<ApplicationPart> parts,
            ControllerFeature feature)
        {
            foreach (var part in parts.OfType<IApplicationPartTypeProvider>())
            {
                foreach (var type in part.Types)
                {
                    if (IsController(type) && !feature.Controllers.Contains(type))
                    {
                        feature.Controllers.Add(type);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if a given <paramref name="typeInfo"/> is a controller.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> candidate.</param>
        /// <returns><code>true</code> if the type is a controller; otherwise <code>false</code>.</returns>
        protected virtual bool IsController(TypeInfo typeInfo)
        {
            if (!typeInfo.IsClass)
            {
                return false;
            }

            if (typeInfo.IsAbstract)
            {
                return false;
            }

            // We only consider public top-level classes as controllers. IsPublic returns false for nested
            // classes, regardless of visibility modifiers
            if (!typeInfo.IsPublic)
            {
                return false;
            }

            if (typeInfo.ContainsGenericParameters)
            {
                return false;
            }

            if (typeInfo.IsDefined(typeof(NonControllerAttribute)))
            {
                return false;
            }

            //如果不是用controller结尾..而且没有定义ControllerAttribute.则不是控制器
            if (!typeInfo.Name.EndsWith(ControllerTypeNameSuffix, StringComparison.OrdinalIgnoreCase) &&
                !typeInfo.IsDefined(typeof(ControllerAttribute)))
            {
                return false;
            }
            return true;
        }
    }
}