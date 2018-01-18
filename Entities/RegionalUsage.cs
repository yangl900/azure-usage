namespace azure_usage_report
{
    using Newtonsoft.Json;

    public class RegionalUsage
    {
        [JsonProperty]
        public string Location { get; set; }

        [JsonProperty]
        public string Resource { get; set; }

        [JsonProperty]
        public int Current { get; set; }

        [JsonProperty]
        public int Limit { get; set; }
    }
}