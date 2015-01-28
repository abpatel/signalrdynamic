using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API.Common
{
    public class Setting
    {
        public int? ID { get; set; }
        public string SystemName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format(@"ID:{0}-System:{1}-{2}({3})", ID, SystemName, Key, Value);
        }
    }
}
