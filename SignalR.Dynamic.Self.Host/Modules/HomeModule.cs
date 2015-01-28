using Nancy;
using Nancy.Responses.Negotiation;
using Nancy.ModelBinding;
using SignalR.Dynamic.API.Common;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.ConsoleHost.Modules
{
    public class HomeModule : NancyModule
    {
        private IRepository<Setting> repo = null;
        private Negotiator GetNegotiator()
        {
            Setting[] model = repo.All.ToArray();
            return Negotiate
                .WithModel(model)
                .WithMediaRangeModel("application/json", model)
                .WithView("Index");
        }

        public HomeModule()
        {

            //this.repo = repo;

            Get["/"] = _ =>
                {
                    return GetNegotiator();
                };
            Get["/createoredit/{id?}"] = _ =>
            {
                Setting setting  = null;
                if (_.id != null)
                {
                    setting = repo.Get(_.id);
                }
                else
                {
                    setting = new Setting();
                }
                return View["CreateOrEdit", setting];
            };

            Post["/createoredit/"] = _ =>
                {
                    var setting = this.Bind<Setting>();
                    this.repo.Add(setting);
                    return Response.AsRedirect("/");
                };
            Post["/delete"] = _ =>
                {
                    var setting = this.Bind<Setting>();
                    repo.Delete(setting.ID.Value);
                    return Response.AsRedirect("/");
                };
        }
    }
}
