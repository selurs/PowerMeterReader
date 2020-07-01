using System.Collections.Generic;

namespace PowerMeterReader.Models
{
    public class RealtimeDataValues
    {
        public string Unit {get;set;}
        public Dictionary<string, int> Values {get;set;}
    }
}
