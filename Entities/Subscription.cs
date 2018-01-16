namespace azure_usage_report
{
    using Newtonsoft.Json;

    public class Subscription
    {
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string SubscriptionId { get; set; }

        [JsonProperty]
        public string DisplayName { get; set; }
        
        [JsonProperty]
        public string State { get; set; }
    }
}