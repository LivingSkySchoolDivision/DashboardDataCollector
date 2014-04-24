using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.Proxy
{
    public partial class JSON : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the URL to actually load from the querystring

            if (!string.IsNullOrEmpty(Request.QueryString["URL"]))
            {
                string UrlToLoad = Request.QueryString["URL"];

                // Attempt to load the requested URL                
                using (WebClient client = new WebClient())
                {
                    byte[] dataReturned = client.DownloadData(UrlToLoad);
                    Response.Clear();
                    Response.ContentType = "application/json; charset=utf-8";
                    Response.BinaryWrite(dataReturned);
                    Response.End();
                }
            }
        }
    }
}