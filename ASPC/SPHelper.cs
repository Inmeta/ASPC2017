using Microsoft.SharePoint.Client;
using System.Security;
using AuthenticationManager = OfficeDevPnP.Core.AuthenticationManager;

namespace ASPC
{
    public class SPHelper
    {
        private static SPHelper _instance;
        private SPHelper() { }
        public static SPHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SPHelper();
                }
                return _instance;
            }
        }

        public ListItemCollection GetListItems(string listname, string url, string username, string password)
        {
            var pwd = new SecureString();
            foreach (var c in password.ToCharArray()) {pwd.AppendChar(c);}
            using (ClientContext clientContext = new ClientContext(url))
            {
                clientContext.Credentials = new SharePointOnlineCredentials(username, pwd);                
                var oList = clientContext.Web.Lists.GetByTitle(listname);
                CamlQuery query = CamlQuery.CreateAllItemsQuery(1000);
                ListItemCollection items = oList.GetItems(query);
                clientContext.Load(items);
                clientContext.ExecuteQuery();
                return items;
            }
        }
    }
}
