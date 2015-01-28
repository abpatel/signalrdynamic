using Newtonsoft.Json;
using SignalR.Dynamic.API.Common;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API
{
    public class JSONMessageSerializer : IMessageSerializer
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            PreserveReferencesHandling = PreserveReferencesHandling.None
        };
    
        public string Serialize(Message message)
        {
            string json = JsonConvert.SerializeObject(message, settings);
            return json;
        }
    }
}
