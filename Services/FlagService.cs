using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CS2ServerPicker.Services
{
    public class FlagService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly Dictionary<string, Image> _flagCache = new Dictionary<string, Image>();

        // City to Country Code mapping
        private readonly Dictionary<string, string> _cityToCountry = new Dictionary<string, string>
        {
            // --- EUROPE ---
            { "Vienna", "at" },
            { "Warsaw", "pl" }, { "Poland", "pl" },
            { "Madrid", "es" }, { "Spain", "es" },
            { "Frankfurt", "de" }, { "Germany", "de" },
            { "London", "gb" },
            { "Amsterdam", "nl" },
            { "Paris", "fr" },
            { "Stockholm", "se" }, { "Kista", "se" }, // Kista e lângă Stockholm
            { "Helsinki", "fi" },

            // --- USA ---
            { "Atlanta", "us" },
            { "Dallas", "us" },         // <--- Asta lipsea!
            { "Chicago", "us" },
            { "Los Angeles", "us" }, { "LA", "us" },
            { "New York", "us" }, { "JFK", "us" },
            { "Seattle", "us" },
            { "Sterling", "us" },       // Washington DC area
            { "Moses Lake", "us" },
            { "Salt Lake City", "us" },
            { "Denver", "us" },
            { "Phoenix", "us" },
            { "San Jose", "us" },
            { "San Diego", "us" },
            { "Washington", "us" },
            { "Ashburn", "us" },        // Virginia
            { "Eatontown", "us" },      // New Jersey
            { "Columbus", "us" },
            { "Saint Louis", "us" },

            // --- ASIA ---
            { "China", "cn" }, { "Shanghai", "cn" }, { "Tianjin", "cn" }, { "Guangzhou", "cn" }, { "Chengdu", "cn" },
            { "Hong Kong", "hk" },
            { "Chennai", "in" }, { "Mumbai", "in" }, { "India", "in" },
            { "Tokyo", "jp" },
            { "Seoul", "kr" },
            { "Singapore", "sg" },

            // --- OTHERS ---
            { "Dubai", "ae" },
            { "Sydney", "au" },
            { "Sao Paulo", "br" }, { "Brazil", "br" },
            { "Santiago", "cl" }, { "Chile", "cl" },
            { "Lima", "pe" }, { "Peru", "pe" },
            { "Buenos Aires", "ar" }, { "Argentina", "ar" },
            { "Johannesburg", "za" } // South Africa
        };

        // Code to Country Name dictionary
        private readonly Dictionary<string, string> _codeToName = new Dictionary<string, string>
        {
            { "at", "Austria" }, { "pl", "Poland" }, { "es", "Spain" },
            { "de", "Germany" }, { "gb", "United Kingdom" }, { "nl", "Netherlands" },
            { "fr", "France" }, { "se", "Sweden" }, { "fi", "Finland" },
            { "cn", "China" }, { "hk", "Hong Kong" }, { "in", "India" },
            { "jp", "Japan" }, { "us", "USA" }, { "br", "Brazil" },
            { "cl", "Chile" }, { "pe", "Peru" }, { "ar", "Argentina" },
            { "au", "Australia" }, { "ae", "UAE" }, { "sg", "Singapore" },
            { "ro", "Romania" }, { "un", "Unknown" }
        };

        public string GetCountryCode(string city)
        {
            foreach (var kvp in _cityToCountry)
            {
                if (city.ToLower().Contains(kvp.Key.ToLower())) return kvp.Value;
            }
            return "un";
        }

        public string GetCountryName(string code)
        {
            if (_codeToName.ContainsKey(code)) return _codeToName[code];
            return code.ToUpper(); // Fallback
        }

        public async Task<Image> GetFlagImageAsync(string countryCode)
        {
            if (countryCode == "un") return null;
            if (_flagCache.ContainsKey(countryCode)) return _flagCache[countryCode];

            try
            {
                // Download small version (w20)
                string url = $"https://flagcdn.com/w20/{countryCode}.png";
                byte[] imageBytes = await _httpClient.GetByteArrayAsync(url);

                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    Image flag = Image.FromStream(ms);
                    _flagCache[countryCode] = flag;
                    return flag;
                }
            }
            catch { return null; }
        }
    }
}