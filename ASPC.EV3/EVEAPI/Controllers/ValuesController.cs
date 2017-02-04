using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EVEAPI.Controllers
{
    public class ValuesController : ApiController
    {
     
        //[HttpPost]
        //public string SendCommand(string command)
        //{
        //    return "dawdwd";
        //}

        // GET api/values/5
        public string Get(string command)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<SignalR.eve3Hub>();
            context.Clients.All.ReceiveString(command);
            return "value";
        }
        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var s = value;

        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
