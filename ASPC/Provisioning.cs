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
        

        /*
         * Method for PnP provisioning.
         */
        public void Provision(string webUrl, string username, string pwd, string environment, string fulldeploy)
        {
            Console.WriteLine("Starting provisioning");

            //Creating securestring password based on input parameter
            var password = new SecureString();
            foreach (var c in pwd.ToCharArray())
            {
                password.AppendChar(c);
            }

            using (var ctx = new ClientContext(webUrl))
            {
                //Setting credentials
                ctx.Credentials = GetCredentials(username, password, environment);

                //Getting web
                var web = ctx.Web;
                ctx.Load(web, w => w.Title);
                if (ctx.WebExistsFullUrl(webUrl))
                {                    
                    ctx.ExecuteQuery();

                    Console.WriteLine("Getting template...");
                    //Getting PnP provisioning template
                    var provider = new XMLFileSystemTemplateProvider(String.Format(@"{0}\..\..\", AppDomain.CurrentDomain.BaseDirectory), string.Empty);
                    var template = provider.GetTemplate("template.xml");

                    Console.WriteLine("Applying template...");
                    //Applying provisioning template
                    web.ApplyProvisioningTemplate(template);
                                        
                    if (fulldeploy == "true")
                    {
                        //Publish files in masterpage gallery
                        Console.WriteLine("Publishing files...");
                        PublishFiles(ctx, webUrl, username, password, environment);

                        //Adding contenttypes to list
                        Console.WriteLine("Adding contenttype to list...");
                        web.AddContentTypeToListById("Pages", "0x010100C568DB52D9D0A14D9B2FDCC96666E9F2007948130EC3DB064584E219954237AF3900DD3A1180E6EA47E596CD0450C5BBF6DD");
                    }
                    else
                    {
                        //Clearing nodes in template.xml file
                        template.ContentTypes.Clear();
                        template.Lists.Clear();
                        template.SiteFields.Clear();
                    }
                }
            }

            Console.WriteLine(String.Format("Finished. Press [Enter] to exit."), true);
            Console.ReadLine();
        }

        /*
         * Setting credential
         * Returns cloud credentials or credentials for on-prem
         */
        public static ICredentials GetCredentials(string uname, SecureString pwd, string environment)
        {
            if (environment == "cloud")
                return new SharePointOnlineCredentials(uname, pwd);
            else
                return new NetworkCredential(uname, pwd);
        }

        /*
         * Method for publishing files in masterpage gallery
         */
        private void PublishFiles(ClientContext clientContext, string UrlPath, string uname, SecureString pwd, string environment)
        {
            var list = clientContext.Web.Lists.GetByTitle("Master Page Gallery");
            var camlQuery = new CamlQuery();
            if (UrlPath.Contains("sites"))
                camlQuery.FolderServerRelativeUrl = string.Format("/{0}/{1}{2}", "sites", UrlPath.Split(new string[] { string.Format("/{0}/", "sites") }, StringSplitOptions.None)[1], "/_catalogs/masterpage/ASPC");
            else
                camlQuery.FolderServerRelativeUrl = "/_catalogs/masterpage/ASPC";
            camlQuery.ViewXml = @"<View Scope='Recursive'><Query><Where><And><Neq><FieldRef Name='_Level'/><Value Type='Integer'>1</Value></Neq><Neq><FieldRef Name='File_x0020_Type' /><Value Type='Text'>js</Value></Neq></And></Where></Query></View>";
            var listItems = list.GetItems(camlQuery);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();
            foreach (var item in listItems)
            {
                item.File.Publish("");
            }

            clientContext.ExecuteQuery();
        }
    }
}

