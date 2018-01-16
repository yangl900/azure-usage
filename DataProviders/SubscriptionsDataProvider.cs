
namespace azure_usage_report
{
    using System.Net.Http;
    using System;
    using System.Threading.Tasks;
    using System.Net;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;

    public class SubscriptionsDataProvider
    {
        private HttpClient Client => new HttpClient();

        private Uri ListSubscriptionsUri => new Uri("https://management.azure.com/subscriptions?api-version=2016-01-01");
        
        public SubscriptionsDataProvider()
        {}

        public async Task<Subscription[]> GetSubscriptions(string accessToken)
        {
            using (var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: this.ListSubscriptionsUri))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await this.Client.SendAsync(request).ConfigureAwait(continueOnCapturedContext: false);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException($"Failed to get subscriptions. Response: '{response.StatusCode}'.");
                }

                var content = await response.Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(continueOnCapturedContext: false);
                
                var tenants = JsonConvert.DeserializeObject<ResponseWithContinuation<Subscription>>(content);

                return tenants.Value;
            }
        }
    }
}