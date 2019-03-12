using Microsoft.Owin;
using System;
using System.Collections.Generic;

namespace DotHass.Middleware.Mvc.Action
{
    /// <summary>
    /// Context object for execution of action which has been selected as part of an HTTP request.
    /// </summary>
    public class ActionContext
    {
        public ActionContext(
            OwinContext context,
            ActionDescriptor actionDescriptor,
           IDictionary<string, object> actionArguments,
           object controller)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (actionDescriptor == null)
            {
                throw new ArgumentNullException(nameof(actionDescriptor));
            }
            if (actionArguments == null)
            {
                throw new ArgumentNullException(nameof(actionArguments));
            }

            ActionArguments = actionArguments;
            Controller = controller;

            MsgContext = context;
            ActionDescriptor = actionDescriptor;
        }

        public ActionDescriptor ActionDescriptor
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Http.HttpContext"/> for the current request.
        /// </summary>
        /// <remarks>
        /// The property setter is provided for unit test purposes only.
        /// </remarks>
        public OwinContext MsgContext
        {
            get; set;
        }

        /// <summary>
        /// Gets the controller instance containing the action.
        /// </summary>
        public virtual object Controller { get; }

        /// <summary>
        /// Gets the arguments to pass when invoking the action. Keys are parameter names.
        /// </summary>
        public virtual IDictionary<string, object> ActionArguments { get; }
    }
}