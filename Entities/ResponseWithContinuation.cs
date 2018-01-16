namespace azure_usage_report
{
    using Newtonsoft.Json;

    public class ResponseWithContinuation<T>
    {
        [JsonProperty]
        public T[] Value { get; set; }

        [JsonProperty]
        public string NextLink { get; set; }
    }
}