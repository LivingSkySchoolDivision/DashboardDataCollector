using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace LSKYDashboardDataCollector.iBoss
{
    public class iBossBandwidthUser
    {
        public string Username { get; set; }
        public string TotalBytesString { get; set; }
        public decimal TotalKBPS
        {
            get
            {
                // Parse the TotalBytesString and get the actual number out of it
                decimal ParsedValue = -1;
                if (decimal.TryParse(GetJustNumbers(TotalBytesString), out ParsedValue))
                {
                    return ParsedValue;
                }
                else
                {
                    return -1;
                }
            }
        }
        public long PacketCount { get; set; }

        private string GetJustNumbers(string inputString)
        {
            char[] numbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.' };

            StringBuilder outputString = new StringBuilder();

            foreach (char c in inputString)
            {
                if (numbers.Contains(c))
                {
                    outputString.Append(c);
                }
            }

            return outputString.ToString();
        }

        public iBossBandwidthUser(string username, string totalbytes, string packetcount)
        {
            this.Username = username;
            this.TotalBytesString = totalbytes;
            this.PacketCount = long.Parse(packetcount);
        }

        public override string ToString()
        {
            return "BandwidthUser { Username: " + this.Username + ", TotalBytes: " + this.TotalBytesString + ", Packets: " + this.PacketCount + " }";
        }

    }
}