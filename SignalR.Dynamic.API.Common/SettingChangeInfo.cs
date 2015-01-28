using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API.Common
{

    public enum ChangeType
    {
        Changed = 0,
        Added,
        Removed
    }
    public class SettingChangeInfo
    {
        public ChangeType ChangeType { get; set; }
        public Setting Setting { get; set; }
    }
}
