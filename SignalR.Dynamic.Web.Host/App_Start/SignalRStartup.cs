using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalR.Dynamic.Web.Host
{
    public class SignalRStartup
    {
        private HubConfiguration hubConfiguration = null;
        public SignalRStartup()
        {
            hubConfiguration = DependencyResolver.Current.GetService<HubConfiguration>();
            hubConfiguration.EnableDetailedErrors = true;
            hubConfiguration.EnableJSONP = true;
        }
        private void InitializeChangeNotifier()
        {
            var notifier = DependencyResolver.Current.GetService<ISettingsChangeNotifier>();
            notifier.Start();
        }
        private void IntializeEventListeners()
        {
            var publishers = DependencyResolver.Current.GetServices<IPublisher>().ToArray();
            Array.ForEach(publishers, p => p.Initialize());
        }

        public void Configuration(IAppBuilder app)
        {
            InitializeChangeNotifier();
            //Ref: http://stackoverflow.com/questions/17457382/windows-authentication-with-signalr-and-owin-self-hosting
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);                
                map.RunSignalR(hubConfiguration);
            });
        }
    }

}