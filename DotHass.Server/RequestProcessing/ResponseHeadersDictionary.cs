// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Specialized;

namespace DotHass.Server.RequestProcessing
{
    // This class exposes the response headers collection as a mutable dictionary, and re-maps restricted headers
    // to their associated properties.
    internal sealed class ResponseHeadersDictionary : HeadersDictionaryBase
    {
        internal ResponseHeadersDictionary()
            : base()
        {
            Headers = new NameValueCollection();
        }
    }
}