using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.iBoss
{
    public class iBossBandwidthUser
    {
        public string Username { get; set; }
        public string TotalBytes { get; set; }
        public long PacketCount { get; set; }

        public iBossBandwidthUser(string username, string totalbytes, string packetcount)
        {
            this.Username = username;
            this.TotalBytes = totalbytes;
            this.PacketCount = long.Parse(packetcount);
        }

        public override string ToString()
        {
            return "BandwidthUser { Username: " + this.Username + ", TotalBytes: " + this.TotalBytes + ", Packets: " + this.PacketCount + " }";
        }

    }
}