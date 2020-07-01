using System.Text.Json.Serialization;

namespace PowerMeterReader.Models
{
    public class RealtimeData
    {
        [JsonPropertyName("DAY_ENERGY")]
        public RealtimeDataValues DayEnergy {get;set;}

        [JsonPropertyName("YEAR_ENERGY")]
        public RealtimeDataValues YearEnergy {get;set;}

        [JsonPropertyName("TOTAL_ENERGY")]
        public RealtimeDataValues TotalEnergy {get;set;}

        [JsonPropertyName("PAC")]
        public RealtimeDataValues InstantaneousPowerGeneration {get;set;}
    }
}
