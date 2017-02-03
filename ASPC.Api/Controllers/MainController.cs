using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;
using System.Web.Http;
using ASPC.Api.Models;
using ASPC;
using Microsoft.SharePoint.Client;

namespace ASPC.Api.Controllers
{
    public class MainController : ApiController
    {
        private static readonly string USER = ConfigurationManager.AppSettings["User"];
        private static readonly string PASS = ConfigurationManager.AppSettings["Pass"];
        private static readonly string BASE = "https://inmetademo.sharepoint.com";

        [HttpPost]
        public string Provision(ProvisionMetadata metadata)
        {
            
            var provisioning = new Provisioning();
            provisioning.Provision(metadata.siteUrl, USER, PASS , "dev", "false");
            return "d";
        }

        [HttpGet]
        public async Task<string> getWebTitle(string endpoint)
        {
            //Creating Password 
            //const string PWD = "softjam.1";
            //const string USER = "bubu@zsis376.onmicrosoft.com";
            //const string RESTURL = "{0}/_api/web?$select=Title";

            //Creating Credentials 
            var passWord = new SecureString();
            foreach (var c in PASS) passWord.AppendChar(c);
            var credential = new SharePointOnlineCredentials(USER, passWord);

            //Creating Handler to allows the client to use credentials and cookie 
            using (var handler = new HttpClientHandler() { Credentials = credential })
            {
                //Getting authentication cookies 
                Uri uri = new Uri(BASE);
                handler.CookieContainer.SetCookies(uri, credential.GetAuthenticationCookie(uri));

                //Invoking REST API 
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(string.Format(endpoint)).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    string jsonData = await response.Content.ReadAsStringAsync();

                    return jsonData;
                }
            }
        }


    }
}
