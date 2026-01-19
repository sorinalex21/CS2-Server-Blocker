using System;
using System.Collections.Generic;
using System.Text;

namespace CS2ServerPicker.Models
{
    public class GameServer
    {
        public string City { get; set; }
        public string IpAddress { get; set; }

        public long Ping { get; set; } = -1; 
        public string CountryCode { get; set; } 

    }
}
