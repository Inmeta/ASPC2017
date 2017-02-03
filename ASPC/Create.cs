using Microsoft.SharePoint.Client;
using System.Security;

namespace ASPC
{
    public class Create
    {
        public static Create _instance;
        public Create() { }
        public static Create Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Create();
                }
                return _instance;
            }
        }

        public Web CreateSiteCollection( string url, string username, string pwd, ClientContext ctx, string title)
        {
            var password = new SecureString();
            foreach (var c in pwd.ToCharArray()) { password.AppendChar(c); }
            using (ClientContext clientContext = new ClientContext(url))
            {
                clientContext.Credentials = new SharePointOnlineCredentials(username, password);
                var w = ctx.Web.CreateWeb(title, url, string.Format("Site for {0}", title), "BLANKINTERNETCONTAINER#0", 1033);                
                return w;
            }
        }
    }
}
