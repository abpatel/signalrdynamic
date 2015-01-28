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
    public interface IHubRepository
    {
        IHub GetHub(string topic, params string[] authorizationRoles);
        IEnumerable<IHub> GetHubs();
    }

    //public interface IHubAssemblyProvider
    //{
    //    public Assembly GetAssembly(string topic);
    //    public IEnumerable<Assembly> GetAssembies();
    //}
}
