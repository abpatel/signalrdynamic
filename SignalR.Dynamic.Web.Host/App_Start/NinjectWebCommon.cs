[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SignalR.Dynamic.Web.Host.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(SignalR.Dynamic.Web.Host.App_Start.NinjectWebCommon), "Stop")]


namespace SignalR.Dynamic.Web.Host.App_Start
{
    using System;
    using System.Web;
    using System.Linq;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using System.Web.Mvc;
    using Ninject.Web.Mvc;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        private static void InitializeSignalRDynamic(IKernel kernel)
        {
            //ref:http://stackoverflow.com/questions/4060528/add-directories-to-asp-net-shadow-copy
            //ref:http://stackoverflow.com/questions/2842208/assemblyresolve-event-is-not-firing-during-compilation-of-a-dynamic-assembly-for
            //ref:http://shazwazza.com/post/Developing-a-plugin-framework-in-ASPNET-with-medium-trust.aspx

            var notifier = kernel.Get<SignalR.Dynamic.API.Interfaces.ISettingsChangeNotifier>();
            var hubRepo = kernel.Get<SignalR.Dynamic.API.Interfaces.IHubRepository>();
            var hubs = hubRepo.GetHubs().ToArray();
            var publishers = kernel.GetAll<SignalR.Dynamic.API.Interfaces.IPublisher>().ToArray();
            notifier.Start();
            Array.ForEach(hubs, h =>
            {
                System.Reflection.Assembly assembly = h.GetHubAssembly();
                System.Web.Compilation.BuildManager.AddReferencedAssembly(assembly);
            });
            Array.ForEach(publishers, p => p.Initialize());

        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load(new SignalRDynamicNinjectModule());
            InitializeSignalRDynamic(kernel);
        }        
    }
}
