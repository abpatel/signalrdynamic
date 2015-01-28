using SignalR.Dynamic.API.Common;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API
{
    public class Relay : IRelay
    {
        private IHubRepository hubRepo = null;
        private IMessageSerializer serializer = null;
        public Relay(IHubRepository hubRepo, IMessageSerializer serializer)
        {
            this.hubRepo = hubRepo;
            this.serializer = serializer;
        }

        public void RelayMessage(Message message)
        {
            try
            {
                var hub = hubRepo.GetHub(message.Topic);
                string serialized = serializer.Serialize(message);
                hub.Invoke(serialized);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                //Log/ bubble up exception;
                throw;
            }
        }
    }
}
