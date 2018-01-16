namespace azure_usage_report
{
    using Newtonsoft.Json;

    public class Usage
    {
        [JsonProperty]
        public string Location { get; set; }

        [JsonProperty]
        public string Unit { get; set; }

        [JsonProperty]
        public int CurrentValue { get; set; }

        [JsonProperty]
        public int Limit { get; set; }
        
        [JsonProperty]
        public UsageName Name { get; set; }
    }

    public class UsageName
    {
        [JsonProperty]
        public string Value { get; set; }

        [JsonProperty]
        public string LocalizedValue { get; set; }
    }
}