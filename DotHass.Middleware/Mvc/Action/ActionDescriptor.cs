using System;
using System.Collections.Generic;

namespace DotHass.Middleware.Mvc.Action
{
    public class ActionDescriptor
    {
        public ActionDescriptor()
        {
            Id = Guid.NewGuid().ToString();
            Properties = new Dictionary<object, object>();
        }

        /// <summary>
        /// Gets an id which uniquely identifies the action.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// A friendly name for this action.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Stores arbitrary metadata properties associated with the <see cref="ActionDescriptor"/>.
        /// </summary>
        public IDictionary<object, object> Properties { get; set; }

        //todo:待扩展..实现验证过滤器等
        //public IList<FilterDescriptor> FilterDescriptors { get; set; }
    }
}