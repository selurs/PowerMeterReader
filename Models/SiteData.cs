using System.Text.Json.Serialization;

namespace PowerMeterReader.Models
{
    public class SiteData
    {
        [JsonPropertyName("E_Day")]
        public double DayEnergy { get; set; }

        [JsonPropertyName("E_Year")]
        public double YearEnergy { get; set; }

        [JsonPropertyName("E_Total")]
        public double TotalEnergy { get; set; }

        [JsonPropertyName("Meter_Location")]
        public string MeterLocation { get; set; }

        public string Mode { get; set; }

        [JsonPropertyName("P_Akku")]
        public double? BatteryThingo { get; set; } // Only used for batteries, doco doesn't actaully say what it really means...

        // The power currently being pulled from the grid. +ve means using grid power, -ve means feeding in
        [JsonPropertyName("P_Grid")]
        public double GridPowerUsage { get; set; }

        // The power currently being drawn off the system. This is always -ve (the house can't *generate* power)
        [JsonPropertyName("P_Load")]
        public double CurrentPowerConsumption { get; set; }

        // The power currently being produced by the solar array
        [JsonPropertyName("P_PV")]
        public double? CurrentPVProduction { get; set; }

        // Percentage of the current power consumption that is being drawn from the PV array
        [JsonPropertyName("rel_Autonomy")]
        public double RelativeAutonomy { get; set; }

        // Percentage of the current production that is being used by the house
        // null if current production is 0 (divide by 0 error)
        [JsonPropertyName("rel_SelfConsumption")]
        public double? RelativeSelfConsumption { get; set; }
    }
}
