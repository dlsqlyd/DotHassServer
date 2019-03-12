// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using DotHass.Server.Abstractions.Message;

namespace DotHass.Server.RequestProcessing
{
    /// <summary>
    /// This wraps ListenerRequest's WebHeaderCollection (NameValueCollection) and adapts it to
    /// the OWIN required IDictionary surface area. It remains fully mutable, but you will be subject
    /// to the header validations performed by the underlying collection.
    /// </summary>
    internal sealed class RequestHeadersDictionary : HeadersDictionaryBase
    {
        private readonly IRequestMessage _request;

        internal RequestHeadersDictionary(IRequestMessage request)
            : base()
        {
            _request = request;

            Headers = _request.Headers;
        }
    }
}