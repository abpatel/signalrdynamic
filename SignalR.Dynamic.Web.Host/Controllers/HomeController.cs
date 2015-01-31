using SignalR.Dynamic.API.Common;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalR.Dynamic.Web.Host.Controllers
{
    public class HomeController : Controller
    {
        private IRepository<Setting> repo = null;
        private IPublisherMetadataProvider metadataProvider = null;

        public HomeController(
            IRepository<Setting> repo, 
            IPublisherMetadataProvider metadataProvider)
        {
            this.repo = repo;
            this.metadataProvider = metadataProvider;
        }
        public ActionResult TopicSettings()
        {
            Setting[] model = repo.All.ToArray();
            return View(model);
        }
       
        public ActionResult Index()
        {
            var metadatas = metadataProvider.GetMetadata();
            return View(metadatas);
        }
        
        public ActionResult CreateOrEdit(int? id)
        {
            Setting setting = null;
            if (id != null)
            {
                setting = repo.Get(id.Value);
            }
            else
            {
                setting = new Setting();
            }
            return View(setting);
        }

        public ActionResult Details(int id)
        {
            Setting setting =  repo.Get(id);
            return View(setting);
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(Setting setting)
        {
            if(setting != null && setting.ID != null)
            {
                this.repo.Delete(setting.ID.Value);
            }
            return RedirectToAction("Index");
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateOrEdit(Setting setting)
        {
            if (setting != null)
            {
                this.repo.Add(setting);
            }
            return RedirectToAction("Index");
        }
    }
}