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
    public class SettingsChangeNotifier : ISettingsChangeNotifier, IDisposable
    {
        private bool disposed = false;
        private IRepository<Setting> repo;
        private IEnumerable<IPublisher> publishers;

        public SettingsChangeNotifier (IRepository<Setting> repo, IEnumerable<IPublisher> publishers)
	    {
            this.repo = repo;
            this.publishers = publishers;
	    }
        public void Start()
        {
 	        repo.OnAdd +=repo_OnAdd;
            repo.OnChange +=repo_OnChange;
            repo.OnRemove +=repo_OnRemove;
        }

        private void Notify(params SettingChangeInfo[] info)
        {
            foreach (var publisher in publishers)
	        {
                try 
	            {	        
            		publisher.OnConfigurationChange(info);
	            }
	            catch (Exception ex)
	            {		
                    //TODO: Log exception here!
                    Debug.WriteLine(ex);
	            }
	        }
        }
        void repo_OnRemove(IEnumerable<Setting> obj)
        {
 	         Notify(obj.Select(s => new SettingChangeInfo{ Setting = s, ChangeType = ChangeType.Removed}).ToArray());
        }

        void repo_OnChange(Setting obj)
        {
 	        Notify(new SettingChangeInfo{ Setting = obj, ChangeType = ChangeType.Changed});
        }

        void repo_OnAdd(Setting obj)
        {
 	        Notify(new SettingChangeInfo{ Setting = obj, ChangeType = ChangeType.Added});
        }

        protected virtual void Dispose(bool disposing)
        {
            if(! disposed)
            {
                if(disposing)
                {
                    //cleanup managed resources
                    if(repo != null)
                    {
                        //TODO: Do we really need to unreister handlers?
                        repo.OnAdd -= repo_OnAdd;
                        repo.OnChange -= repo_OnChange;
                        repo.OnRemove -= repo_OnRemove;
                    }
                }
            }
        }
        public void Dispose()
        {
 	        Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
