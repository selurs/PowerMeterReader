using System.Collections.Generic;

namespace PowerMeterReader.Models
{
    public class PowerFlowRealtimeData
    {
        public string Version { get; set; }
        public Dictionary<string, InverterData> Inverters { get; set; }
        public SiteData Site { get; set; }
    }
}
