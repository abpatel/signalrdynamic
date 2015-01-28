using SignalR.Dynamic.API.Common;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API.Interfaces
{
    public interface IPublisher
    {
        void Initialize();
        void Publish();
        void OnConfigurationChange(params SettingChangeInfo[] settingChangeInfos);
    }
}
