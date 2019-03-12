// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace DotHass.Server.RequestProcessing
{
    internal class ListenerStreamWrapper : ExceptionFilterStream
    {
        internal ListenerStreamWrapper(Stream innerStream)
            : base(innerStream)
        {
        }

        // Convert ListenerExceptions to IOExceptions
        protected override bool TryWrapException(Exception ex, out Exception wrapped)
        {
            wrapped = null;
            return false;
        }

        public override void Close()
        {
            // Disabled. The server will close the response when the AppFunc task completes.
        }

        [SuppressMessage("Microsoft.Usage", "CA2215:Dispose methods should call base class dispose", Justification = "By design")]
        protected override void Dispose(bool disposing)
        {
            // Disabled. The server will close the response when the AppFunc task completes.
        }
    }
}