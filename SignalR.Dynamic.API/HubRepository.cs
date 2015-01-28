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
    public class HubRepository : IHubRepository
    {
        IHubCodeGenerator codegen = null;
        ICodeDOMCompiler compiler = null;
        IPublisherMetadataProvider provider = null;
        Dictionary<string, IHub> maps = new Dictionary<string, IHub>();

        public HubRepository(
            IHubCodeGenerator codegen,
            ICodeDOMCompiler compiler,
            IPublisherMetadataProvider provider)
        {
            this.codegen = codegen;
            this.compiler = compiler;
            this.provider = provider;
            InitializeMaps();
        }

        private void InitializeMaps()
        {
            foreach (var metadata in provider.GetMetadata())
            {
                IHub hub = GetHubInternal(metadata.Topic, metadata.AuthorizationRoles);
                if (! maps.ContainsKey(metadata.Topic)) //First hub with name = systemName wins!(poor man's de-dupe)
                {
                    maps[metadata.Topic] = hub;
                }
            }
        }

        public IHub GetHub(string topic, params string[] authorizationRoles)
        {
            if (!maps.ContainsKey(topic))
            {
                throw new InvalidOperationException(string.Format("No hub found for systemName = {0}", topic));
            }
            return maps[topic];
        }

        public IEnumerable<IHub> GetHubs()
        {
            return maps.Values;
        }

        private IHub GetHubInternal(string topic, string[] authorizationRoles)
        {
            var compilationUnit = codegen.GenerateCodeDOM(topic, authorizationRoles);
            Assembly assembly = compiler.Compile(compilationUnit);
            Type hubType = assembly.GetTypes()
                .First(t => ! t.IsAbstract && 
                t.GetInterfaces()
                .Contains(typeof(SignalR.Dynamic.API.Interfaces.IHub)));
            IHub hubInstance = Activator.CreateInstance(hubType) as IHub;
            return hubInstance;
        }
    }
}
