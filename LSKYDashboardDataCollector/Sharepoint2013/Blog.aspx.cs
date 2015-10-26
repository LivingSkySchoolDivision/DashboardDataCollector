using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.Sharepoint2013
{
    public partial class Blog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Parse data from the querystring
            string baseURL = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["url"]))
            {
                baseURL = Request.QueryString["url"].ToString().Trim();
            }



        }
    }
}