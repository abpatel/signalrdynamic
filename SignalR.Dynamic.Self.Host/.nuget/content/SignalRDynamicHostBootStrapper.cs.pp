using System;
using SignalR.Dynamic.Self.Host;

namespace $rootnamespace$
{
    public class SignalRDynamicHostBootStrapper
    {
        public void Start()
        {
            using (SignalRDynamicHost host = new SignalRDynamicHost(new SignalRDynamicHostConfig
                                            {
                                                AdminUrl = new Uri("http://localhost:9876"),
                                                HostUrl = new Uri("http://localhost:8192"),
                                                SettingsFileName = "settings.json"
                                            }
                                      )
                    )
            {
                host.Start();
                System.Console.ReadLine();
            }
        }
    }
}
