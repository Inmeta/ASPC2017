using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ZionGraph.Common
{
    public class AuthenticationHelper
    {
        public static string TokenForUser;
        public static string TokenForApplication;

        /// <summary>
        /// Get Active Directory Client for Application.
        /// </summary>
        /// <returns>ActiveDirectoryClient for Application.</returns>
        public static ActiveDirectoryClient GetActiveDirectoryClientAsApplication()
        {
            
            Uri servicePointUri = new Uri(GlobalConstants.ResourceUrl);
            Uri serviceRoot = new Uri(servicePointUri, AppModeConstants.TenantId);
            ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                async () => await AcquireTokenAsyncForApplication());
            return activeDirectoryClient;
        }

        /// <summary>
        /// Async task to acquire token for Application.
        /// </summary>
        /// <returns>Async Token for application.</returns>
        public static async Task<string> AcquireTokenAsyncForApplication()
        {
            return await GetTokenForApplication();
        }

        /// <summary>
        /// Get Token for Application.
        /// </summary>
        /// <returns>Token for application.</returns>
        public static async Task<string> GetTokenForApplication()
        {
            //if (TokenForApplication == null)
            //{
            AuthenticationContext authenticationContext = new AuthenticationContext(AppModeConstants.AuthString, false);
            // Config for OAuth client credentials 
            ClientCredential clientCred = new ClientCredential(AppModeConstants.ClientId,
                AppModeConstants.ClientSecret);
            AuthenticationResult authenticationResult =
                await authenticationContext.AcquireTokenAsync(GlobalConstants.ResourceUrl,
                    clientCred);
            TokenForApplication = authenticationResult.AccessToken;
            //}
            var url = await authenticationContext.GetAuthorizationRequestUrlAsync(GlobalConstants.ResourceUrl, AppModeConstants.ClientId, new Uri("https://inmetademo.sharepoint.com"), UserIdentifier.AnyUser, "prompt=admin_consent");
            Console.WriteLine(url);

            return TokenForApplication;
        }

        public static async Task<string> GetTokenForSharePoint()
        {

            var tenantAdminUri = new Uri("https://inmetademo.sharepoint.com");
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            
            var tokenResponse =  TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
            //AuthenticationContext authenticationContext = new AuthenticationContext(AppModeConstants.AuthString, false);
            //// Config for OAuth client credentials 
            //ClientCredential clientCred = new ClientCredential("b03215cc-9dda-46f2-8112-9232c60056c2",
            //    "QDkxb0vzMj/pxudSb5BnA15670ZGQgmnm2BfcIPN4XY=");
            //AuthenticationResult authenticationResult =
            //    await authenticationContext.AcquireTokenAsync(GlobalConstants.ResourceUrlSharePoint,
            //        clientCred);
            
            //TokenForApplication = authenticationResult.AccessToken;
            //}
            

            return tokenResponse;
        }
        /// <summary>
        /// Get Active Directory Client for User.
        /// </summary>
        /// <returns>ActiveDirectoryClient for User.</returns>
        public static ActiveDirectoryClient GetActiveDirectoryClientAsUser()
        {
            Uri servicePointUri = new Uri(GlobalConstants.ResourceUrl);
            Uri serviceRoot = new Uri(servicePointUri, UserModeConstants.TenantId);
            ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                async () => await AcquireTokenAsyncForUser());
            return activeDirectoryClient;
        }

        /// <summary>
        /// Async task to acquire token for User.
        /// </summary>
        /// <returns>Token for user.</returns>
        public static async Task<string> AcquireTokenAsyncForUser()
        {
            return await GetTokenForUser();
        }

        /// <summary>
        /// Get Token for User.
        /// </summary>
        /// <returns>Token for user.</returns>
        public static async Task<string> GetTokenForUser()
        {
            if (TokenForUser == null)
            {
                var redirectUri = new Uri("https://localhost");
                AuthenticationContext authenticationContext = new AuthenticationContext(UserModeConstants.AuthString, false);
                AuthenticationResult userAuthnResult = await authenticationContext.AcquireTokenAsync(GlobalConstants.ResourceUrl,
                    UserModeConstants.ClientId, redirectUri, new PlatformParameters(PromptBehavior.RefreshSession));
                TokenForUser = userAuthnResult.AccessToken;
                Console.WriteLine("\n Welcome " + userAuthnResult.UserInfo.GivenName + " " +
                                  userAuthnResult.UserInfo.FamilyName);
            }
            return TokenForUser;
        }

        public async static Task doStuffInOffice365()
        {
            //set the authentication context
            string authority = "https://login.windows.net/inmetaaspc2017.onmicrosoft.com/";
            AuthenticationContext authenticationContext = new AuthenticationContext(authority, false);

            //read the certificate private key from the executing location
            var certPath = AppDomain.CurrentDomain.BaseDirectory;
            certPath = certPath.Substring(0, certPath.LastIndexOf('\\')) + "\\ZionZentral.pfx"; // The Cert you uploaded to Azure AD application
            var certfile = System.IO.File.OpenRead(certPath);
            var certificateBytes = new byte[certfile.Length];
            certfile.Read(certificateBytes, 0, (int)certfile.Length);

            var cert = new X509Certificate2(
                certificateBytes,
                "ZionZentral",
                X509KeyStorageFlags.Exportable |
                X509KeyStorageFlags.MachineKeySet |
                X509KeyStorageFlags.PersistKeySet);
            ClientAssertionCertificate cac = new ClientAssertionCertificate("b03215cc-9dda-46f2-8112-9232c60056c2", cert);

            //get the access token to SharePoint using the ClientAssertionCertificate
            Console.WriteLine("Getting app-only access token to SharePoint Online");
            try
            {
                var authenticationResult = await authenticationContext.AcquireTokenAsync("https://inmetaaspc2017.sharepoint.com/", cac); // Do not use the site "https:// mycompany.sharepoint.com/sites/MyTestSite"
                                                                                                                                     // your code 
                var token = authenticationResult.AccessToken;
                Console.WriteLine("App-only access token retreived");

                //perform a post using the app-only access token to add SharePoint list item in Attendee list
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

                const string siteURL = "https://mycompany.sharepoint.com/sites/MyTestSite";

                // Get list from title
                const string listAction = siteURL + "/_api/web/lists/getbytitle('TestList')";
                HttpResponseMessage listResult = await client.GetAsync(listAction).ConfigureAwait(false);
                listResult.EnsureSuccessStatusCode();
                string listJsonData = await listResult.Content.ReadAsStringAsync();
                Console.WriteLine(listJsonData);

                // Get all items from list
                const string itemAction = siteURL + "/_api/web/lists/getbytitle('TestList')/items";
                HttpResponseMessage response1 = await client.GetAsync(itemAction).ConfigureAwait(false);
                response1.EnsureSuccessStatusCode();
                string itemJsonData = await response1.Content.ReadAsStringAsync();
                Console.WriteLine(itemJsonData);
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e);
            }


        }

    }
}
