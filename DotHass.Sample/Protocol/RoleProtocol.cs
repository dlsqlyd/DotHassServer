using DotHass.Sample.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHass.Sample.Protocol
{
    [Serializable]
    public class RoleProtocol
    {
        public long roleid { get; set; }
        public string name { get; set; }
        public string headid { get; set; }
        public int money { get; set; }
        public int gold { get; set; }
        public int exp { get; set; }

        public GangConfig gang { get; set; }
    }
}
