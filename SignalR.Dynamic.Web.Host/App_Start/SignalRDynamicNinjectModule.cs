using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Extensions.Conventions;
using SignalR.Dynamic.API.Common;
using SignalR.Dynamic.API.Interfaces;
using SignalR.Dynamic.API;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using Ninject;
using System.Web.Compilation;
using System.Reflection;

namespace SignalR.Dynamic.Web.Host
{
    public class SignalRDynamicNinjectModule : NinjectModule
    {
        private void InitializeNinject()
        {
            string settingsFileName = ConfigurationManager.AppSettings["SettingsFileName"];
            string fullyQualifiedSettingFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), settingsFileName);
            this.Bind<IRepository<Setting>>()
                .To<SettingsRepository>()
                .InSingletonScope()
                .WithConstructorArgument("settingsFileName", fullyQualifiedSettingFilePath);
            this.Bind(_ =>
            {
                _.FromAssembliesMatching("SignalR.Dynamic.Client.*.dll", "SignalR.Dynamic.API.dll")
                    .SelectAllClasses().Excluding<SettingsRepository>()
                    .BindAllInterfaces()
                    .Configure(b => b.InSingletonScope());
            });

            this.Bind<HubConfiguration>().To<HubConfiguration>()
                .InSingletonScope()
                .WithPropertyValue("Resolver", new NinjectSignalRDependencyResolver(this.Kernel));
        }

        private void InitializeSignalRDynamic()
        {
            //ref:http://stackoverflow.com/questions/4060528/add-directories-to-asp-net-shadow-copy
            //ref:http://stackoverflow.com/questions/2842208/assemblyresolve-event-is-not-firing-during-compilation-of-a-dynamic-assembly-for
            //ref:http://shazwazza.com/post/Developing-a-plugin-framework-in-ASPNET-with-medium-trust.aspx

            var notifier = this.Kernel.Get<ISettingsChangeNotifier>();
            var hubRepo = this.Kernel.Get<IHubRepository>();
            var hubs = hubRepo.GetHubs().ToArray();
            var publishers = this.Kernel.GetAll<IPublisher>().ToArray();
            notifier.Start();
            Array.ForEach(hubs, h => 
                {
                    Assembly assembly = h.GetHubAssembly();
                    BuildManager.AddReferencedAssembly(assembly);
                });            
            Array.ForEach(publishers, p => p.Initialize());
           
        }
        public override void Load()
        {
            InitializeNinject();
            //InitializeSignalRDynamic();
        }
    }
}