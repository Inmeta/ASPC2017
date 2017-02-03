using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ASPC
{
    public class Create
    {
        public void CreateSiteCollection(Web web, string url, string username, SecureString pwd, ClientContext ctx, string title)
        {
            web.CreateWeb(title, url, string.Format("Site for {0}", title), "BLANKINTERNETCONTAINER#0", 1033);
        }
    }
}
