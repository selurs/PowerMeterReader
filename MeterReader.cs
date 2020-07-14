using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PowerMeterReader.Models;

namespace PowerMeterReader
{
    public class MeterReader
    {
        private readonly string _ipAddress;
        private static readonly HttpClient _client = new HttpClient();

        public MeterReader(string ipAddress)
        {
            _ipAddress = ipAddress;
        }

        public async Task<RealtimeDataModel> ReadInverterData(string scope)
        {
            return await ReadData<RealtimeDataModel>($"{Definitions.GET_INVERTER_DATA_METHOD}?{Definitions.GET_DATA_SCOPE_PARAM}={scope}");
        }

        public async Task<PowerFlowRealtimeDataModel> ReadPowerFlowData()
        {
            return await ReadData<PowerFlowRealtimeDataModel>(Definitions.GET_POWER_FLOW_DATA_METHOD);
        }

        private async Task<T> ReadData<T>(string uri)
        {
            using (var response = await _client.GetAsync($"http://{_ipAddress}{uri}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    var realtimeData = await JsonSerializer.DeserializeAsync<T>(apiResponse);
                    return realtimeData;
                }

                return default(T);
            }
        }
    }
}
