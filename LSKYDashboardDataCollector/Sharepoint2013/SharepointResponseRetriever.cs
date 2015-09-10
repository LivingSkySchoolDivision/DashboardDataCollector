using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace LSKYDashboardDataCollector.Sharepoint2013
{
    public class SharepointResponseRetriever
    {
        public static Stream GetSharepointResponse(string username, string password, string siteurl)
        {
            // Build the URL
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(siteurl);
            request.Credentials = new System.Net.NetworkCredential(username, password);
            request.Accept = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            request.UserAgent = "Sharepoint Dashboard Connector, written by Mark Strendin, Living Sky School Division";
            WebResponse webresponse = request.GetResponse();
            return webresponse.GetResponseStream();
        }

        public static Stream GetSharepointResponse(string username, string password, string sitebase, string guid)
        {
            string RSSURL = sitebase + "/_layouts/15/listfeed.aspx?List=" + guid;
            return GetSharepointResponse(username, password, RSSURL);
        }
    }
}