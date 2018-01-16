
namespace azure_usage_report
{
    using System.Net.Http;
    using System;
    using System.Threading.Tasks;
    using System.Net;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;

    public class LocationsDataProvider
    {
        private HttpClient Client => new HttpClient();

        private Uri ListLocationsUri => new Uri("https://management.azure.com/locations?api-version=2016-01-01");
        
        public LocationsDataProvider()
        {}

        public async Task<Location[]> GetLocations(string accessToken)
        {
            using (var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: this.ListLocationsUri))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await this.Client.SendAsync(request).ConfigureAwait(continueOnCapturedContext: false);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException($"Failed to get locations. Response: '{response.StatusCode}'.");
                }

                var content = await response.Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(continueOnCapturedContext: false);
                
                var locations = JsonConvert.DeserializeObject<ResponseWithContinuation<Location>>(content);

                return locations.Value;
            }
        }
    }
}