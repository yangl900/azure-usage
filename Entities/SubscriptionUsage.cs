namespace azure_usage_report
{
    using Newtonsoft.Json;

    public class SubscriptionUsage
    {
        [JsonProperty]
        public Subscription Subscription { get; set; }

        [JsonProperty]
        public Usage[] Usages { get; set; }
    }
}