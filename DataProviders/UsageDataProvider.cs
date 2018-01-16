
namespace azure_usage_report
{
    using System.Net.Http;
    using System;
    using System.Threading.Tasks;
    using System.Net;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;

    public class UsageDataProvider
    {
        private HttpClient Client => new HttpClient();

        private const string ComputeUsageUriTemplate = "https://management.azure.com/subscriptions/{0}/providers/Microsoft.Compute/locations/{1}/usages?api-version=2017-12-01";
        
        public UsageDataProvider()
        {}

        public async Task<Usage[]> GetComputeUsage(string accessToken, string subscriptionId, string location)
        {
            var requestUri = string.Format(UsageDataProvider.ComputeUsageUriTemplate, subscriptionId, location);
            using (var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: requestUri))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await this.Client.SendAsync(request).ConfigureAwait(continueOnCapturedContext: false);

                var content = await response.Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(continueOnCapturedContext: false);
                
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException($"Failed to get compute usage. Response: '{response.StatusCode}': '{content}'.");
                }
                
                var usages = JsonConvert.DeserializeObject<ResponseWithContinuation<Usage>>(content);

                return usages.Value;
            }
        }
    }
}