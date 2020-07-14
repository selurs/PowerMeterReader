using System.Text.Json.Serialization;

namespace PowerMeterReader.Models
{
    public class InverterData
    {
        [JsonPropertyName("DT")]
        public int DeviceTypeId { get; set; }

        [JsonPropertyName("E_Day")]
        public double DayEnergy { get; set; }

        [JsonPropertyName("E_Year")]
        public double YearEnergy { get; set; }

        [JsonPropertyName("E_Total")]
        public double TotalEnergy { get; set; }

        [JsonPropertyName("P")]
        public double InstantaneousPowerGeneration { get; set; }
    }
}
