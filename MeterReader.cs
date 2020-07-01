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

        public async Task<RealtimeDataModel> Read(string scope)
        {
            using (var response = await _client.GetAsync($"http://{_ipAddress}{Definitions.GET_DATA_METHOD}?{Definitions.GET_DATA_SCOPE_PARAM}={scope}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    var realtimeData = await JsonSerializer.DeserializeAsync<RealtimeDataModel>(apiResponse);
                    return realtimeData;
                }

                return null;
            }
        }
    }
}
