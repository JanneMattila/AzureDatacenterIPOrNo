using Newtonsoft.Json;

namespace IpCheck
{
    public class IpValidationResult
    {
        [JsonProperty(PropertyName = "region", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Region { get; set; }

        [JsonProperty(PropertyName = "ipRange", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string IpRange { get; set; }

        [JsonProperty(PropertyName = "ip", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Ip { get; set; }

        [JsonProperty(PropertyName = "error", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Error { get; set; }
    }    
}
