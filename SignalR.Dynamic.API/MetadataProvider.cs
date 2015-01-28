using SignalR.Dynamic.API.Common;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API
{
    public class PublisherMetadataProvider : IPublisherMetadataProvider
    {
        private Lazy<IEnumerable<Metadata>> metadata = null;

        public PublisherMetadataProvider()
        {
            metadata = new Lazy<IEnumerable<Metadata>>(
                () => GetMetadataInternal(),
                LazyThreadSafetyMode.PublicationOnly);
        }

        private IEnumerable<Metadata> GetMetadataInternal()
        {
            string path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            var publishers =
                Directory.GetFiles(path, "SignalR.Dynamic*.dll")
                .SelectMany(f => Assembly.LoadFrom(f).GetTypes())
                .Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(IPublisher)));

            IEnumerable<Metadata> metadata =
                publishers.Select(e => e.GetCustomAttribute<MetadataAttribute>())
                .Select(a => new Metadata
                {
                    Topic = a.SystemName,
                    AuthorizationRoles = !string.IsNullOrWhiteSpace(a.Roles) ?
                                            a.Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { }
                }
                );
            return metadata;
        }
        public IEnumerable<Metadata> GetMetadata()
        {
            return metadata.Value;
        }
    }
}
