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

            Sharepoint2013BlogParser parser = new Sharepoint2013BlogParser();
            List<SharepointBlogPost> blogPosts = parser.ParseRSSFeed(Settings.SharePointUsername, Settings.SharePointPassword, baseURL);

            foreach (SharepointBlogPost post in blogPosts)
            {
                Response.Write("<BR><b>" + post.Title + "</b>: " + post.Content);
            }


        }
    }
}