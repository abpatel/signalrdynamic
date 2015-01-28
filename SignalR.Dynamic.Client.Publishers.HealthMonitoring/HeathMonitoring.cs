using SignalR.Dynamic.API.Common;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Dynamic.Client.Publishers
{
    [Metadata(Topic = "HealthMonitor")]
    public class HeathMonitoring : IPublisher
    {
        private PerformanceCounter cpuCounter;
        private Timer t = null;
        private IRelay relay = null;
        private string machineName = null;
        private IConfiguration configuration = null;

        public HeathMonitoring(IRelay relay, IConfiguration configuration)
        {
            this.relay = relay;
            this.configuration = configuration;
        }

        private void InitializePerformanceCounter()
        {
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            cpuCounter.MachineName = this.machineName;
            cpuCounter.NextValue();
        }
        public void Initialize()
        {
            Debug.WriteLine("Initializing Health Monitor Event Listener");
            var config = configuration.GetConfiguration().Where(s => s.Topic == "HealthMonitor").ToArray();
            Debug.WriteLine("Configuration");
            Array.ForEach(config, c => Debug.WriteLine(c));
            var setting = config.FirstOrDefault(c => c.Key == "machinename");
            this.machineName = setting != null ?  setting.Value : Environment.MachineName;
            InitializePerformanceCounter();
        }
        public void Publish()
        {
            //simulate some events
            t = new Timer(_ =>
            {
                relay.RelayMessage(new Message
                {
                    Topic = "HealthMonitor",
                    Properties = new
                    {
                        machineName = machineName,
                        CPUPercentage = cpuCounter.NextValue()
                    }
                });
            }, null, 0, 3000);
        }
        public void OnConfigurationChange(params SettingChangeInfo[] settingChangeInfos)
        {
            var changedSettings = settingChangeInfos.Where(s => s.Setting.Topic == "HealthMonitor");
            foreach (var setting in changedSettings)
            {
                Debug.WriteLine("{0} was {1}", setting.Setting.Key, setting.ChangeType);
            }

        }

    }
}
