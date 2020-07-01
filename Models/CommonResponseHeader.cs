using System;
using System.Text.Json.Serialization;

namespace PowerMeterReader.Models
{
    public class CommonResponseHeader
    {
        [JsonPropertyName("Timestamp")]
        public DateTime TimeStamp {get;set;}
        public ResponseStatus Status {get;set;}
        public RequestArguments RequestArguments {get;set;}
    }
}
