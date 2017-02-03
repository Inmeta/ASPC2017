using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using ZionGraph.Common;
using System.Text;
using Newtonsoft.Json;

namespace ZionGraph.Controllers
{
    public class GraphController : ApiController
    {
        public async Task<Token> Get()
        {
            string token = string.Empty;

            token = await AuthenticationHelper.AcquireTokenAsyncForApplication();

            var results = new Token {token = token};
            
            return results;
        }

        // GET: api/Example/5
        public string Get(string id)
        {
            return "value";
        }
    }


    public class Token
    {
        public string token { get; set; }
    }
}