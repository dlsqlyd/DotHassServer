// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Owin.Hosting
{
    public static class Constants
    {
        public const string HostTraceOutput = "host.TraceOutput";
        public const string HostTraceSource = "host.TraceSource";
        public const string HostOnAppDisposing = "host.OnAppDisposing";
        public const string HostAddresses = "host.Addresses";
        public const string HostAppName = "host.AppName";
        public const string HostAppMode = "host.AppMode";
        public const string AppModeDevelopment = "development";

        public const string OwinServerFactory = "OwinServerFactory";
        public const string SettingsOwinServer = "owin:Server";
        public const string EnvOwnServer = "OWIN_SERVER";
        public const string DefaultServer = "Microsoft.Owin.Host.HttpListener";
        public const string SettingsPort = "owin:Port";
        public const string EnvPort = "PORT";
        public const int DefaultPort = 5000;

        public const string SettingsOwinAppStartup = "owin:AppStartup";

        public const string Scheme = "scheme";
        public const string Host = "host";
        public const string Port = "port";
        public const string Path = "path";
    }
}