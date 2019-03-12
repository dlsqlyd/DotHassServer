// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Owin.Hosting.Builder;
using Owin;
using Owin.Loader;
using System;
using System.Collections.Generic;

namespace Microsoft.Owin.Hosting.Loader
{
    using AppLoaderFunc = Func<string, IList<string>, Action<IAppBuilder>>;

    /// <summary>
    /// Initializes a new app loader.
    /// </summary>
    public class AppLoaderFactory : IAppLoaderFactory
    {
        private readonly IAppActivator _activator;

        /// <summary>
        ///
        /// </summary>
        /// <param name="activator"></param>
        public AppLoaderFactory(IAppActivator activator)
        {
            _activator = activator;
        }

        /// <summary>
        /// Not currently used.
        /// </summary>
        public virtual int Order
        {
            get { return -100; }
        }

        /// <summary>
        /// Create a new chained app loader.
        /// </summary>
        /// <param name="nextLoader"></param>
        /// <returns></returns>
        public virtual AppLoaderFunc Create(AppLoaderFunc nextLoader)
        {
            var loader = new DefaultLoader(nextLoader, _activator.Activate);
            return loader.Load;
        }
    }
}