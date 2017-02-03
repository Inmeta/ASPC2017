namespace ZionGraph.Common
{
    internal class AppModeConstants
    {
        //https://login.microsoftonline.com/bahrda.onmicrosoft.com/FederationMetadata/2007-06/FederationMetadata.xml


        public const string ClientId = "590ed5bb-5ccc-46d3-8527-0be1fb44083f";
        public const string ClientSecret = "dQ4uDHx2rAGR1G7RpbH2LuaspHEoRZ2sRschesdGjuE=";
        public const string TenantName = "inmetaaspc2017.onmicrosoft.com";
        public const string TenantId = "7b0ff2cc-2e19-444c-9158-c692bf834492";
        public const string AuthString = GlobalConstants.AuthString + TenantName;
    }

    internal class UserModeConstants
    {
        public const string TenantId = AppModeConstants.TenantId;
        public const string ClientId = "66133929-66a4-4edc-aaee-13b04b03207d";
        public const string AuthString = GlobalConstants.AuthString + "common/";
    }

    internal class GlobalConstants
    {
        public const string AuthString = "https://login.microsoftonline.com/";
        public const string ResourceUrl = "https://graph.windows.net";
        public const string GraphServiceObjectId = "00000002-0000-0000-c000-000000000000";
    }
}
