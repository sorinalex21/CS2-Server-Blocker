using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CS2ServerPicker.Services
{
    public class FirewallService
    {
        private List<string> _blockedCities = new List<string>();
        private const string RULE_PREFIX = "CS2_BLOCK_";

        public FirewallService() { }

        // Get the current list of blocked cities (from cache or refresh)
        public List<string> GetBlockedCities()
        {
            // Refresh list directly from Windows every time requested
            return ReadRulesFromWindows();
        }

        public Dictionary<string, int> GetBlockedIpCounts()
        {
            var counts = new Dictionary<string, int>();
            try
            {
                // We will use netsh to get all rules and parse their RemoteIPs
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "netsh.exe",
                    Arguments = "advfirewall firewall show rule name=all",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    string currentRuleName = "";
                    
                    foreach (string line in lines)
                    {
                        string l = line.Trim();
                        if (l.StartsWith("Rule Name:"))
                        {
                            currentRuleName = l.Substring("Rule Name:".Length).Trim();
                        }
                        else if (l.StartsWith("RemoteIP:"))
                        {
                            if (!currentRuleName.Contains(RULE_PREFIX)) continue;

                            string ips = l.Substring("RemoteIP:".Length).Trim();
                            if (ips == "Any") continue;

                            // Extract city name from rule name
                            string cityKey = currentRuleName.Replace(RULE_PREFIX, "");
                            cityKey = Regex.Replace(cityKey, @"_\d+$", ""); // Remove index suffix
                            
                            // Count IPs
                            int count = ips.Split(',').Length;

                            if (counts.ContainsKey(cityKey))
                                counts[cityKey] += count;
                            else
                                counts[cityKey] = count;
                        }
                    }
                }
            }
            catch { }
            return counts;
        }

        public void BlockCity(string cityName, string ipFromApi)
        {
            if (string.IsNullOrEmpty(cityName)) return;

            // 1. Get ranges
            List<string> rangesToBlock = new List<string>();
            if (SteamApiService.CityRanges.ContainsKey(cityName))
            {
                rangesToBlock = SteamApiService.CityRanges[cityName];
            }
            else
            {
                rangesToBlock.Add(ConvertToSubnet(ipFromApi));
            }

            // 2. Batch Command
            StringBuilder batchCommand = new StringBuilder();
            string baseName = $"{RULE_PREFIX}{cityName.Replace(" ", "")}";

            // Delete old rules
            batchCommand.Append($"netsh advfirewall firewall delete rule name=\"{baseName}\" & ");
            for (int i = 0; i < 15; i++)
            {
                batchCommand.Append($"netsh advfirewall firewall delete rule name=\"{baseName}_{i}\" & ");
            }

            // Add new rules
            int index = 0;
            foreach (string range in rangesToBlock)
            {
                string ruleName = $"{baseName}_{index}";
                batchCommand.Append($"netsh advfirewall firewall add rule name=\"{ruleName}\" dir=out action=block remoteip={range} enable=yes & ");
                index++;
            }

            RunBatchCommand(batchCommand.ToString());
        }

        public void DeleteRule(string cityName)
        {
            StringBuilder batchCommand = new StringBuilder();
            string baseName = $"{RULE_PREFIX}{cityName.Replace(" ", "")}";

            batchCommand.Append($"netsh advfirewall firewall delete rule name=\"{baseName}\" & ");
            for (int i = 0; i < 15; i++)
            {
                batchCommand.Append($"netsh advfirewall firewall delete rule name=\"{baseName}_{i}\" & ");
            }

            RunBatchCommand(batchCommand.ToString());
        }

        public void DeleteAllRulesVitezaLuminis()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = $"-Command \"Remove-NetFirewallRule -DisplayName '{RULE_PREFIX}*' -ErrorAction SilentlyContinue\"",
                    Verb = "runas",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process.Start(psi)?.WaitForExit();
            }
            catch { }
        }

        // --- READING FROM WINDOWS ---
        private List<string> ReadRulesFromWindows()
        {
            List<string> foundCities = new List<string>();

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "netsh.exe",
                Arguments = "advfirewall firewall show rule name=all",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            try
            {
                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string line in lines)
                    {
                        if (line.Contains(RULE_PREFIX))
                        {
                            int index = line.IndexOf(RULE_PREFIX);
                            if (index >= 0)
                            {
                                string rawName = line.Substring(index + RULE_PREFIX.Length).Trim();
                                // Clean name (remove suffixes like _0, _1)
                                string cleanName = Regex.Replace(rawName, @"_\d+$", "");
                                if (!foundCities.Contains(cleanName))
                                {
                                    foundCities.Add(cleanName);
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return foundCities;
        }

        private string ConvertToSubnet(string ip)
        {
            try
            {
                var parts = ip.Split('.');
                if (parts.Length == 4) return $"{parts[0]}.{parts[1]}.{parts[2]}.0/24";
            }
            catch { }
            return ip;
        }

        private void RunBatchCommand(string longCommand)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/C {longCommand}";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.Verb = "runas";
                p.Start();
                p.WaitForExit();
            }
            catch { }
        }
        public void BatchUpdateRules(List<string> citiesToBlock, List<string> citiesToUnblock, Dictionary<string, string> cityIpMap)
        {
            StringBuilder netshScript = new StringBuilder();

            // Unblocks
            foreach (string city in citiesToUnblock)
            {
                string baseName = $"{RULE_PREFIX}{city.Replace(" ", "")}";
                
                // Netsh script lines don't need 'netsh' prefix, just the context command
                // However, 'netsh -f' expects commands as if typed in interactive mode.
                // Depending on context, safe bet is full path from root if not entering context.
                // "advfirewall firewall delete rule ..." works from root.
                
                netshScript.AppendLine($"advfirewall firewall delete rule name=\"{baseName}\"");
                // Cleanup indexed/split ones
                for(int i=0; i<5; i++) 
                {
                   netshScript.AppendLine($"advfirewall firewall delete rule name=\"{baseName}_{i}\"");
                }
            }

            // Blocks
            foreach (string city in citiesToBlock)
            {
                if (!cityIpMap.ContainsKey(city)) continue;

                string ip = cityIpMap[city];
                List<string> rangesToBlock = new List<string>();
                
                if (SteamApiService.CityRanges.ContainsKey(city))
                {
                    rangesToBlock = SteamApiService.CityRanges[city];
                }
                else
                {
                    rangesToBlock.Add(ConvertToSubnet(ip));
                }

                if (rangesToBlock.Count == 0) continue;

                string baseName = $"{RULE_PREFIX}{city.Replace(" ", "")}";
                
                // Clean old
                netshScript.AppendLine($"advfirewall firewall delete rule name=\"{baseName}\"");
                for (int i = 0; i < 5; i++) netshScript.AppendLine($"advfirewall firewall delete rule name=\"{baseName}_{i}\"");

                // Add New - Single Rule with joined IPs
                string joinedRanges = string.Join(",", rangesToBlock);
                netshScript.AppendLine($"advfirewall firewall add rule name=\"{baseName}\" dir=out action=block remoteip={joinedRanges} enable=yes");
            }

            if (netshScript.Length > 0)
            {
                RunNetshScript(netshScript.ToString());
            }
        }

        private void RunNetshScript(string scriptContent)
        {
            string tempFile = System.IO.Path.GetTempFileName();
            try
            {
                // 'netsh' does not like spaces in file paths sometimes? 
                // It handles them with quotes usually.
                // But safer to move to a simple path if possible, but temp is fine.
                System.IO.File.WriteAllText(tempFile, scriptContent);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = $"-f \"{tempFile}\"",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process.Start(psi)?.WaitForExit();
            }
            catch { }
            finally
            {
                try { System.IO.File.Delete(tempFile); } catch { }
            }
        }

        // Keep existing single methods for compatibility or context menu usage if needed, 
        // but ideally we should route them to batch too or keep them for single actions.
        // For now, I'll leave them as is.

        // ... existing methods ...
        // (Just ensure the closing brace is correct)
    }
}