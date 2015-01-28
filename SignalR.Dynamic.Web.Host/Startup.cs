using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Web.Mvc;
using System.Linq;
using Microsoft.Owin.Cors;

[assembly: OwinStartupAttribute(typeof(SignalR.Dynamic.Web.Host.Startup))]
namespace SignalR.Dynamic.Web.Host
{

    public partial class Startup
    {

        private void InitializeSignalRDynamic()
        {
            var notifier = DependencyResolver.Current.GetService<ISettingsChangeNotifier>();
            var publishers = DependencyResolver.Current.GetServices<IPublisher>().ToArray();
            notifier.Start();
            Array.ForEach(publishers, p => p.Initialize());
        }

        private void StartSignalRDynamicPublishers()
        {
            var publishers = DependencyResolver.Current.GetServices<IPublisher>().ToArray();
            Array.ForEach(publishers, p => p.Publish());
        }      
        private void ConfigureSignalR(IAppBuilder app)
        {
            //http://stackoverflow.com/questions/25614794/how-does-the-class-referred-to-by-the-owinstartup-attribute-get-called
            HubConfiguration hubConfiguration = new HubConfiguration(); // DependencyResolver.Current.GetService<HubConfiguration>();
            hubConfiguration.EnableDetailedErrors = true;
            hubConfiguration.EnableJSONP = true;
            hubConfiguration.EnableJavaScriptProxies = true;
            //Ref: http://stackoverflow.com/questions/17457382/windows-authentication-with-signalr-and-owin-self-hosting
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                map.RunSignalR(hubConfiguration);
            });
        }
        public void Configuration(IAppBuilder app)
        {
            //InitializeSignalRDynamic();
            ConfigureSignalR(app);
            //ConfigureAuth(app);
            StartSignalRDynamicPublishers();
        }
    }
}
