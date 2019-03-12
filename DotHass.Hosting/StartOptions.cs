using System;
using System.Collections.Generic;

namespace DotHass.Hosting
{
    /// <summary>
    /// Settings to control the startup behavior of an OWIN application
    /// </summary>
    [Serializable]
    public class StartOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartOptions"/> class
        /// </summary>
        public StartOptions()
        {
            Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Parameter to locate and load web application startup routine
        /// </summary>
        public string AppStartup { get; set; }

        /// <summary>
        /// Name of the assembly containing the http server implementation
        /// </summary>
        public string ServerFactory { get; set; }

        /// <summary>
        /// Optional settings used to override service types and other defaults
        /// </summary>
        public IDictionary<string, string> Settings { get; set; }
    }
}