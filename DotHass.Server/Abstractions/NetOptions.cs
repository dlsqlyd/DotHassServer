using System.Collections.Generic;

namespace DotHass.Server.Abstractions
{
    public class NetOptions
    {
        public int BossLoopCount { get; set; } = 1;

        public int WorkLoopCount { get; set; } = 0;

        public List<ServerOptions> Servers { get; set; } = new List<ServerOptions>();
    }
}