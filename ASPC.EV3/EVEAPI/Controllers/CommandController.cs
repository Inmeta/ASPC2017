using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace EVEAPI.Controllers
{
    public class CommandController : System.Web.Http.ApiController
    {
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("Send")]
        public string Send(string command)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<SignalR.eve3Hub>();
            context.Clients.All.ReceiveString(command);
            return "val";
        }
    }
}
