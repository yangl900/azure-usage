namespace azure_usage_report
{
    using Newtonsoft.Json;

    public class ErrorResponseMessage
    {
        [JsonProperty]
        public ErrorInfo Error { get; set; }
    }
}