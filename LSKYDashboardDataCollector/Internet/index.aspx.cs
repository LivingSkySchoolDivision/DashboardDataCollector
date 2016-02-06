using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.CommunityNet
{
    public partial class Internet : System.Web.UI.Page
    {
        /// Connect to a website. If the website connection worked, then the internet connection is working - if not, then there may be issues
        
        private string BoolToYesOrNo(bool thisBool)
        {
            if (thisBool)
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }

        private int BoolToOneOrZero(bool thisBool) 
        {
            if (thisBool) 
            {
                return 1;
            } 
            else 
            {
                return 0;
            }
        }

        private bool CanAccessWebsite(string url)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.AllowAutoRedirect = true;

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (WebException ex)
            {
                // 401 errors create an exception, but that still indicates that the site works, and thats all I care about here.
                if (ex.Message.Contains("401"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {   
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{");

            Response.Write("\"Google\" : " + BoolToOneOrZero(CanAccessWebsite("http://www.google.com")) + ", ");
            Response.Write("\"Microsoft\" : " + BoolToOneOrZero(CanAccessWebsite("http://www.microsoft.com")) + ", ");
            Response.Write("\"Amazon\" : " + BoolToOneOrZero(CanAccessWebsite("http://www.amazon.com")) + ", ");
            Response.Write("\"LSKYWWW\" : " + BoolToOneOrZero(CanAccessWebsite("http://www.lskysd.ca")) + ", ");
            Response.Write("\"LSKYPortal\" : " + BoolToOneOrZero(CanAccessWebsite("https://portal.lskysd.ca")) + ", ");
            Response.Write("\"LSKYHelpDesk\" : " + BoolToOneOrZero(CanAccessWebsite("https://helpdesk.lskysd.ca")) + " ");

            Response.Write("}");
            Response.End();          

        }
    }
}