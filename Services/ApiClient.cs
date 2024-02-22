using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

using System.Text.Json;
using Newtonsoft.Json;

namespace ApiClient
{
    internal class MullvadApiClient
    {
        internal HttpClient ApiClient { get; set; }

        internal MullvadApiClient()
        {
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
        [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
        internal async Task<object> GetRelays()
        {
            MullvadApiClient client = new();
            Uri uri = new("https://api.mullvad.net/app/v1/relays");
            using HttpResponseMessage responseMessage = await client.ApiClient.GetAsync(uri);
            string response = string.Empty;
            if (responseMessage.IsSuccessStatusCode)
            {
                response = await responseMessage.Content.ReadAsStringAsync();

                MullvadRelaysResponse mullResponse = JsonConvert.DeserializeObject<MullvadRelaysResponse>(response);

                //using StreamWriter sw = new(@"c:\Users\Ari\Desktop\list");
                //JsonSerializer serializer = new();
                //Regex.Unescape()
                //serializer.Serialize(sw, response);


            }
            return response ?? null;
        }
    }

    public class MullvadRelaysResponse
    {
        public List<MullvadRelay> Relays { get; set; }
        // You can add more properties if the JSON contains additional information

        // If the JSON response has a root element, you may need to adjust the class accordingly
    }

    public class MullvadRelay
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Hostname { get; set; }
        public string IP { get; set; }
        public int WireGuardPort { get; set; }
        // Add more properties as needed based on the actual JSON structure
    }
}