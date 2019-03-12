// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace DotHass.Server.Abstractions
{
    public class ServerOptions
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool IsSsl { get; set; }
        public int MaxConnection { get; set; }
        public int Timeout { get; set; }

        public string CertificatePath { get; set; }
        public string CertificateToken { get; set; }
    }
}