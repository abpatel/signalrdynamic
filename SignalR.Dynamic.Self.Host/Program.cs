using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using Nancy.Conventions;
using Nancy.Hosting.Self;
using Ninject;
using Ninject.Extensions.Conventions;
using Owin;
using SignalR.Dynamic.API;
using SignalR.Dynamic.API.Common;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Linq;
using System.Net;

namespace SignalR.Dynamic.Self.Host
{
    public class SignalRDynamicHostConfig
    {
        public Uri HostUrl
        {
            get;
            set;
        }
        public Uri AdminUrl
        {
            get;
            set;
        }
        public string SettingsFileName
        {
            get;
            set;
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Ref: http://stackoverflow.com/questions/17457382/windows-authentication-with-signalr-and-owin-self-hosting
            app.Map("/signalr", map =>
                {
                    var listener = (HttpListener)app.Properties[typeof(HttpListener).FullName];
                    listener.AuthenticationSchemes = AuthenticationSchemes.Ntlm;

                    map.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
                    var hubConfiguration = new HubConfiguration
                    {
                        EnableDetailedErrors = true,
                        EnableJSONP = true
                    };
                    map.RunSignalR(hubConfiguration);
                });
        }
    }
    public class SignalRDynamicHost : IDisposable
    {
        private bool disposed = false;
        //private NancyHost nancyHost = null;
        //private IDisposable signalRHost = null;
        private IDisposable host = null;
        private SignalRDynamicHostConfig config = null;
        private static StandardKernel kernel = new StandardKernel();

        private void InitializeNinject()
        {
            kernel.Bind<IRepository<Setting>>()
                .To<SettingsRepository>()
                .InSingletonScope()
                .WithConstructorArgument("settingsFileName", config.SettingsFileName);
            kernel.Bind(_ =>
            {
                _.FromAssembliesMatching("SignalR.Dynamic.Client.*.dll", "SignalR.Dynamic.API.dll")
                    .SelectAllClasses().Excluding<SettingsRepository>()
                    .BindAllInterfaces()
                    .Configure(b => b.InSingletonScope());
            });
        }

        public class Bootstrapper : NinjectNancyBootstrapper
        {

            protected override IKernel GetApplicationContainer()
            {
                kernel.Load<FactoryModule>();
                return kernel;
            }

            protected override void ConfigureConventions(NancyConventions nancyConventions)
            {
                base.ConfigureConventions(nancyConventions);
                nancyConventions.StaticContentsConventions.Add(
                    StaticContentConventionBuilder.AddDirectory("scripts", @"scripts", "js")
                    );
                nancyConventions.StaticContentsConventions.Add(
                    StaticContentConventionBuilder.AddDirectory("css", @"css", "css")
                    );
            }

            protected override void ApplicationStartup(IKernel container, IPipelines pipelines)
            {
                base.ApplicationStartup(container, pipelines);
            }


            protected override IKernel CreateRequestContainer(Nancy.NancyContext context)
            {
                return kernel;
            }
        }

        public SignalRDynamicHost(SignalRDynamicHostConfig config)
        {
            this.config = config;
        }

        private void InitializeChangeNotifier()
        {
            var notifier = kernel.Get<ISettingsChangeNotifier>();
            notifier.Start();
        }
        private void IntializeEventListeners()
        {
            var generators = kernel.GetAll<IPublisher>().ToArray();
            Array.ForEach(generators, g => g.Initialize());
        }

        private void InitializeSignalRDynamic()
        {
            var notifier = kernel.Get<ISettingsChangeNotifier>();
            var publishers = kernel.GetAll<IPublisher>().ToArray();
            notifier.Start();
            Array.ForEach(publishers, p => p.Initialize());
        }

        private void StartSignalRDynamicPublishers()
        {
            var publishers = kernel.GetAll<IPublisher>().ToArray();
            Array.ForEach(publishers, p => p.Publish());
        }  

        //public void Start()
        //{
        //    if (disposed)
        //    {
        //        throw new ObjectDisposedException("This instance has been disposed");
        //    }
        //    InitializeNinject();
        //    nancyHost = new NancyHost(
        //        new HostConfiguration
        //        {
        //            UrlReservations = new UrlReservations
        //            {
        //                CreateAutomatically = true
        //            }
        //        }, config.AdminUrl); //setup Nancy Host
        //    nancyHost.Start();
        //    Console.WriteLine("Nancy Server running on {0}", config.AdminUrl.AbsoluteUri);
        //    signalRHost = WebApp.Start(config.HostUrl.AbsoluteUri); //setup signalR self host
        //    Console.WriteLine("SignalR.Dynamic Server running on {0}", config.HostUrl.AbsoluteUri);
        //    InitializeChangeNotifier();
        //    IntializeEventListeners();
        //}

        public void Start()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("This instance has been disposed");
            }
            //InitializeNinject();
            //InitializeSignalRDynamic();
            var options = new StartOptions();
            options.Urls.Add(this.config.HostUrl.AbsoluteUri);
            host = WebApp.Start(options,
                builder =>
                {

                    builder.Properties["host.AppName"] = "Self host";
#if DEBUG
                    builder.Properties["host.AppMode"] = "development";
#endif

                    builder.MapSignalR("/signalr",
                        new HubConfiguration
                        {
                            EnableJSONP = true,
                            EnableJavaScriptProxies = true,
                            Resolver = new DefaultDependencyResolver()
                        });
                    builder.UseErrorPage();
                    builder.UseNancy();
                   // StartSignalRDynamicPublishers();
                });
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    //if (signalRHost != null)
                    //{
                    //    signalRHost.Dispose();
                    //}
                    //if (nancyHost != null)
                    //{
                    //    nancyHost.Dispose();
                    //}
                    if (host != null)
                    {
                        host.Dispose();
                    }
                    if (kernel != null)
                    {
                        kernel.Dispose();
                    }
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        static void Main(string[] args)
        {
            //http://www.strathweb.com/2013/05/the-future-is-now-owin-and-multi-hosting-asp-net-web-applications/
            //http://www.asp.net/aspnet/overview/owin-and-katana/an-overview-of-project-katana
            //http://www.gregpakes.co.uk/post/owin-series-part-2-katana
            //http://www.asp.net/signalr/overview/guide-to-the-api/hubs-api-guide-javascript-client
            using (SignalRDynamicHost host = new SignalRDynamicHost(new SignalRDynamicHostConfig
                                            {
                                                AdminUrl = new Uri("http://localhost:9876"),
                                                HostUrl = new Uri("http://localhost:8192"),
                                                SettingsFileName = "settings.json"
                                            }
                                       )
                                       )
            {
                Console.Write("Starting host...");
                host.Start();
                Console.WriteLine("done!");
                Console.ReadLine();
            }
        }
    }
}
