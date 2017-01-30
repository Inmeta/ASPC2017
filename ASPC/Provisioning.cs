using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Net;
using System.Security;

namespace ASPC
{
    public class Provisioning
    {
        private static Provisioning _instance;
        private Provisioning() { }
        public static Provisioning Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Provisioning();
                }
                return _instance;
            }
        }
        public void Provision(string webUrl, string username, string pwd, string environment)
        {
            Console.WriteLine("Starting provisioning");
            var password = new SecureString();
            foreach (var c in pwd.ToCharArray())
            {
                password.AppendChar(c);
            }
            using (var ctx = new ClientContext(webUrl))
            {
                ctx.Credentials = GetCredentials(username, password, environment);
                var web = ctx.Web;
                ctx.Load(web, w => w.Title);
                if (ctx.WebExistsFullUrl(webUrl))
                {
                    Console.WriteLine("Checking...");
                    ctx.ExecuteQuery();
                    var provider = new XMLFileSystemTemplateProvider(String.Format(@"{0}\..\..\", AppDomain.CurrentDomain.BaseDirectory), string.Empty);
                    var template = provider.GetTemplate("template.xml");
                    web.ApplyProvisioningTemplate(template);
                }
            }

            Console.WriteLine(String.Format("Finished. Press [Enter] to exit."), true);
            Console.ReadLine();
        }
        public static ICredentials GetCredentials(string uname, SecureString pwd, string environment)
        {
            if (environment == "cloud")
                return new SharePointOnlineCredentials(uname, pwd);
            else
                return new NetworkCredential(uname, pwd);
        }
    }
}

