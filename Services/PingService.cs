using System;
using System.Collections.Generic;
using System.Text;

using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace CS2ServerPicker.Services
{
    public class PingService
    {
        public async Task<long> GetLatencyAsync(string ipAddress)
        {
            try
            {
                using (Ping pinger = new Ping())
                {
                    // Send ping with 1s timeout
                    PingReply reply = await pinger.SendPingAsync(ipAddress, 1000);

                    if (reply.Status == IPStatus.Success)
                    {
                        return reply.RoundtripTime;
                    }
                }
            }
            catch
            {
                // Ignore errors (return 999 if server is down)
            }

            return 999;
        }
    }
}
