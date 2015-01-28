using Newtonsoft.Json;
using SignalR.Dynamic.API.Common;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API
{
    public class SettingsRepository : IRepository<Setting>
    {
        //private List<Setting> settings = new List<Setting>
        //{
        //    new Setting
        //    {
        //         ID = 1,
        //         Key = "HostName",
        //         SystemName = "HealthMonitor",
        //         Value = "localhost"
        //    }
        //};
        private Lazy<List<Setting>> lazySettings = null;
        private string settingsFileName = null;

        private List<Setting> Initialize()
        {
            if(File.Exists(settingsFileName))
            {
                string s = File.ReadAllText(this.settingsFileName);
                var deserializedSettings = JsonConvert.DeserializeObject<List<Setting>>(s);
                return deserializedSettings;
            }
            return new List<Setting>();
        }

        private void SaveSettings()
        {
            var s = JsonConvert.SerializeObject(this.lazySettings.Value);
            File.WriteAllText(this.settingsFileName, s);
        }
        public SettingsRepository(string settingsFileName)
        {
            this.settingsFileName = settingsFileName;
            this.lazySettings = new Lazy<List<Setting>>(Initialize, LazyThreadSafetyMode.PublicationOnly);
        }
        public Setting Get(int id)
        {
            return this.lazySettings.Value.FirstOrDefault(s => s.ID == id);
        }

        public void Add(Setting t)
        {
            if (t.ID.HasValue && this.lazySettings.Value.Exists(s => s.ID == t.ID.Value))
            {
                Setting setting = this.lazySettings.Value.First(s => s.ID == t.ID.Value);
                setting.Value = t.Value;
                SaveSettings();
                if(OnChange != null)
                {
                    Action<Setting> action = OnChange;
                    action(setting);
                }
            }
            else
            {
                int id = this.lazySettings.Value.Count > 0 ? this.lazySettings.Value.Max(s => s.ID.Value) + 1 : 1;
                t.ID = id;
                this.lazySettings.Value.Add(t);
                SaveSettings();
                if(OnAdd != null)
                {
                    Action<Setting> action = OnAdd;
                    action(t);
                }
            }
        }

        public void Delete(int id)
        {
            var settingsToRemove = this.lazySettings.Value.FindAll(s => s.ID == id);
            if(settingsToRemove.Any())
            {
                this.lazySettings.Value.RemoveAll(s => s.ID == id);
                if(OnRemove != null)
                {
                    Action<IEnumerable<Setting>> action = OnRemove;
                    action(settingsToRemove);
                }
            }
            SaveSettings();
        }

        public IEnumerable<Setting> All
        {
            get { return this.lazySettings.Value ; }
        }

        public event Action<Setting> OnAdd;

        public event Action<Setting> OnChange;

        public event Action<IEnumerable<Setting>> OnRemove;
    }
}
