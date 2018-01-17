
namespace azure_usage_report
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    public static class AuthUtil
    {
        private const string Resource = "https://management.azure.com/";

        public static AuthenticationResult GetToken(string tenantId, string clientId, string clientSecret)
        {
            LoggerCallbackHandler.UseDefaultLogging = false;
            var ctx = new AuthenticationContext("https://login.microsoftonline.com/" + tenantId);
            var credential = new ClientCredential(clientId, clientSecret);

            AuthenticationResult result = null;
            try
            {
                result = ctx.AcquireTokenAsync(AuthUtil.Resource, credential).Result;
            }
            catch (Exception ex)
            {
                var adalEx = ex.InnerException as AdalException;
                if (adalEx != null)
                {
                    Trace.TraceInformation($"Error code: {adalEx.ErrorCode}. Messsage: {adalEx.Message}.");
                }
                else
                {
                    throw;
                }
            }

            return result;
        }
    }
}