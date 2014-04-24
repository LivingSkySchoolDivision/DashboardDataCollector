using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.Proxy
{
    public class CachedWebRequest
    {
        public byte[] Data { get; set; }
        public string URL { get; set; }
        public DateTime LastUpdated { get; set; }

        public CachedWebRequest(string url, byte[] data)
        {
            this.URL = url;
            this.Data = data;
            this.LastUpdated = DateTime.Now;
        }
    }
}