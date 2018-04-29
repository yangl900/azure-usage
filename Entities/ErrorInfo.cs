namespace azure_usage_report
{
    using Newtonsoft.Json;

    public class ErrorInfo
    {
        [JsonProperty]
        public string Code { get; set; }

        [JsonProperty]
        public string Message { get; set; }
    }
}