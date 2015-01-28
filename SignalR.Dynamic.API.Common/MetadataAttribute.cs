using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class MetadataAttribute : Attribute
    {
        public string Topic { get; set; }
        public string Roles { get; set; }
    }
}
