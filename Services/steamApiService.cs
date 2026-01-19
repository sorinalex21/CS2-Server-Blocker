using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using CS2ServerPicker.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace CS2ServerPicker.Services
{
    public class SteamApiService
    {
        private const string SDR_API_URL = "https://api.steampowered.com/ISteamApps/GetSDRConfig/v1/?appid=730";
        private readonly HttpClient _httpClient;

        public static Dictionary<string, List<string>> CityRanges = new Dictionary<string, List<string>>();

        // GLOBAL Translation Dictionary
        private readonly Dictionary<string, string> _popToCity = new Dictionary<string, string>
        {
            // --- CHINA (Perfect World / Alibaba Cloud) ---
            { "pw",   "China (Perfect World)" },
            { "pwu",  "Hebei (China) [Unicom]" },
            { "pwt",  "Hebei (China) [Telecom]" },
            { "pek",  "Beijing (China)" },
            { "pekt", "Beijing (China) [Telecom]" },
            { "peku", "Beijing (China) [Unicom]" },
            { "sha",  "Shanghai (China)" },
            { "shat", "Shanghai (China) [Telecom]" },
            { "shau", "Shanghai (China) [Unicom]" },
            { "pvgt", "Shanghai (China) [Alibaba-Telecom]" },
            { "pvgu", "Shanghai (China) [Alibaba-Unicom]" },
            { "can",  "Guangzhou (China)" },
            { "cant", "Guangzhou (China) [Telecom]" },
            { "canu", "Guangzhou (China) [Unicom]" },
            { "tsn",  "Tianjin (China)" },
            { "tsnt", "Tianjin (China) [Telecom]" },
            { "tsnu", "Tianjin (China) [Unicom]" },
            { "ctu",  "Chengdu (China)" },
            { "ctut", "Chengdu (China) [Telecom]" },
            { "ctuu", "Chengdu (China) [Alibaba-Unicom]" },
            { "hgh",  "Hangzhou (China)" },
            { "pekm", "Alibaba Cloud Beijing - Mobile (China)" },
            { "pwg", "Perfect World Guangdong 1 (China)" },
            { "tgdt", "Tencent Guangzhou - Telecom (China)" },
            { "pvgm", "Alibaba Cloud Shanghai - Mobile (China)" },
            { "tgdm", "Tencent Guangzhou - Mobile (China)" },
            { "ctum", "Alibaba Cloud Chengdu - Mobile (China)" },
            { "shb", "Perfect World (sha-4) Backbone (Shanghai, China)" },
            { "pwz", "Perfect World Zhejiang (China)" },
            //{ "tgdu", "Tencent Guangzhou - Unicom (China)" },



            // --- EUROPE ---
            { "ams", "Amsterdam (Netherlands)" },
            { "fra", "Frankfurt (Germany)" },
            { "lhr", "London (UK)" },
            { "mad", "Madrid (Spain)" },
            { "par", "Paris (France)" },
            { "sto", "Stockholm (Sweden)" },
            { "vie", "Vienna (Austria)" },
            { "waw", "Warsaw (Poland)" },
            { "lux", "Luxembourg" },
            { "hel", "Helsinki (Finland)" },

            // --- NORTH AMERICA ---
            { "atl", "Atlanta (US South)" },
            { "ord", "Chicago (US Central)" },
            { "lax", "Los Angeles (US West)" },
            { "jfk", "New York (US East)" },
            { "eat", "Seattle (US North West)" },
            { "iad", "Sterling/DC (US East)" },
            { "sea", "Seattle (US West)" },
            { "dfw", "Dallas (US Central)" },
            { "phx", "Phoenix (US West)" },

            // --- SOUTH AMERICA ---
            { "gru", "Sao Paulo (Brazil)" },
            { "scl", "Santiago (Chile)" },
            { "lim", "Lima (Peru)" },
            { "eze", "Buenos Aires (Argentina)" },
            { "bog", "Bogota (Colombia)" },

            // --- ASIA & REST OF WORLD ---
            { "bom", "Mumbai (India)" },
            { "maa", "Chennai (India)" },
            { "dxb", "Dubai (UAE)" },
            { "hkg", "Hong Kong" },
            { "sgp", "Singapore" },
            { "tyo", "Tokyo (Japan)" },
            { "seo", "Seoul (South Korea)" },
            { "syd", "Sydney (Australia)" },
            { "mel", "Melbourne (Australia)" },
            { "jnb", "Johannesburg (South Africa)" },
            { "cpt", "Cape Town (South Africa)" }
        };

        public SteamApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<GameServer>> GetRelaysAsync()
        {
            CityRanges.Clear();
            var servers = new List<GameServer>();

            try
            {
                string json = await _httpClient.GetStringAsync(SDR_API_URL);

                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("pops", out var pops))
                    {
                        foreach (var pop in pops.EnumerateObject())
                        {
                            try
                            {
                                string originalCode = pop.Name;

                                // Clean digits (ex: sto2 -> sto)
                                string cleanCode = Regex.Replace(originalCode, @"[\d-]", "");

                                string apiDescription = "";
                                if (pop.Value.TryGetProperty("desc", out var descElem))
                                {
                                    apiDescription = descElem.GetString();
                                }

                                string cityName;
                                // Priority 1: Exact code match
                                if (_popToCity.ContainsKey(originalCode))
                                {
                                    cityName = _popToCity[originalCode];
                                }
                                // Priority 2: Cleaned code match
                                else if (_popToCity.ContainsKey(cleanCode))
                                {
                                    cityName = _popToCity[cleanCode];
                                }
                                // Priority 3: API Description
                                else if (!string.IsNullOrEmpty(apiDescription))
                                {
                                    cityName = apiDescription;
                                }
                                // Priority 4: Total fallback
                                else
                                {
                                    cityName = $"{cleanCode.ToUpper()} (Server)";
                                }

                                if (pop.Value.TryGetProperty("relays", out var relays))
                                {
                                    bool cityAlreadyExists = CityRanges.ContainsKey(cityName);
                                    if (!cityAlreadyExists) CityRanges[cityName] = new List<string>();

                                    string firstIp = "";

                                    foreach (var relay in relays.EnumerateArray())
                                    {
                                        if (relay.TryGetProperty("ipv4", out var ipElem))
                                        {
                                            string ip = ipElem.GetString();
                                            if (string.IsNullOrEmpty(firstIp)) firstIp = ip;

                                            string subnet = ConvertToSubnet(ip);
                                            if (!CityRanges[cityName].Contains(subnet))
                                            {
                                                CityRanges[cityName].Add(subnet);
                                            }
                                        }
                                    }

                                    if (!cityAlreadyExists && CityRanges[cityName].Count > 0)
                                    {
                                        servers.Add(new GameServer { City = cityName, IpAddress = firstIp });
                                    }
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("API Error: " + ex.Message);
            }

            return servers;
        }

        private string ConvertToSubnet(string ip)
        {
            var parts = ip.Split('.');
            if (parts.Length == 4) return $"{parts[0]}.{parts[1]}.{parts[2]}.0/24";
            return ip;
        }
    }
}