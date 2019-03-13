using System;
using System.Collections.Generic;
using System.Text;

namespace DotHass.Sample.Protocol
{
    [Serializable]
    public class UserProtocol
    {
        public string UserId { get; set; }

        public object RetailData { get; set; }
    }
}
