using SignalR.Dynamic.API.Common;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API
{
    public class HubAssemblyProvider : IHubAssemblyProvider
    {

        IHubCodeGenerator codegen = null;
        ICodeDOMCompiler compiler = null;
        IPublisherMetadataProvider provider = null;
        Dictionary<string, Assembly> maps = new Dictionary<string, Assembly>();

        public HubAssemblyProvider(
            IHubCodeGenerator codegen,
            ICodeDOMCompiler compiler,
            IPublisherMetadataProvider provider)
        {
            this.codegen  = codegen;
            this.compiler = compiler;
            this.provider = provider;
            InitializeMaps();
        }

        private void InitializeMaps()
        {
            foreach (var metadata in provider.GetMetadata())
            {
                if (! maps.ContainsKey(metadata.Topic))
                {
                    maps[metadata.Topic] = null;
                }
            }
        }
       
        public Assembly GetAssembly(string topic)
        {
            throw new NotImplementedException();

        }

        public IEnumerable<Assembly> GetAssembies()
        {
            throw new NotImplementedException();
        }
    }
}
