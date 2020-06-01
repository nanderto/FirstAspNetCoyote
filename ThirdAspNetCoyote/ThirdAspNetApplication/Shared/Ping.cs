using System;
using System.Collections.Generic;
using System.Text;

namespace ThirdAspNetApplication.Shared
{
    public class Ping
    {
        public DateTimeOffset TimeOfPingOnServer => DateTimeOffset.Now;
    }
}
