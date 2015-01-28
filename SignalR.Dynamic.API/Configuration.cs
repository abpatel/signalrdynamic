using SignalR.Dynamic.API.Common;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.Dynamic.API
{
    public class Configuration : IConfiguration
    {
        IRepository<Setting> repo = null;

        public Configuration(IRepository<Setting> repo)
        {
            this.repo = repo;
        }
        public IEnumerable<Setting> GetConfiguration()
        {
            return repo.All;
        }
    }
}
