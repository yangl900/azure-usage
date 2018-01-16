namespace azure_usage_report
{
    using Newtonsoft.Json;

    public class Location
    {
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string DisplayName { get; set; }
    }
}