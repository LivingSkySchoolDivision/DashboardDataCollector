using LSKYDashboardDataCollector.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.Proxy
{
    public partial class XML : System.Web.UI.Page
    {

        // Content type to return
        private string MimeType = "text/xml";

        // Cache
        private static Dictionary<string, CachedWebRequest> RequestCache;
        private TimeSpan CacheLifetime = new TimeSpan(0, 10, 0);

        /// <summary>
        /// Gets the specified URL, from the cache if possible
        /// </summary>
        /// <param name="URL">URL address to get data from</param>
        /// <param name="SkipCache">Skip caching and grab live data</param>
        /// <returns></returns>
        private byte[] GetWebData(string URL, bool SkipCache)
        {
            // Initialize the cache if required
            if (RequestCache == null)
            {
                RequestCache = new Dictionary<string, CachedWebRequest>();
            }

            // Hash the URL so we can refer to it later
            string URLHash = CommonFunctions.CalculateMD5Hash(URL);

            if (!SkipCache)
            {
                if (RequestCache.ContainsKey(URLHash))
                {
                    if (DateTime.Now.Subtract(RequestCache[URLHash].LastUpdated) <= CacheLifetime)
                    {
                        return RequestCache[URLHash].Data;
                    }
                    else
                    {
                        RequestCache.Remove(URLHash);
                    }
                }
            }
            // Get the request from the web
            using (WebClient client = new WebClient())
            {
                byte[] dataReturned = client.DownloadData(URL);

                if (dataReturned.Length > 0)
                {
                    if (!SkipCache)
                    {
                        RequestCache.Add(URLHash, new CachedWebRequest(URL, dataReturned));
                    }

                    return dataReturned;
                }
            }

            return new byte[0];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["URL"]))
            {
                // Parse the requested URL
                string UrlToLoad = Request.QueryString["URL"];

                bool SkipCache = false;
                if (!string.IsNullOrEmpty(Request.QueryString["SKIPCACHE"]))
                {
                    if (Request.QueryString["SKIPCACHE"].ToLower() == "yes")
                    {
                        SkipCache = true;
                    }
                }

                Response.Clear();
                Response.ContentType = MimeType;
                byte[] ReturnedData = GetWebData(UrlToLoad, SkipCache);
                if (ReturnedData.Length > 0)
                {
                    Response.BinaryWrite(ReturnedData);
                }
                Response.End();


            }
        }
    }
}