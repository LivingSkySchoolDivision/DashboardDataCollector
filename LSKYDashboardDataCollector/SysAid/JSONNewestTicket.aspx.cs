using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LSKYDashboardDataCollector.Common;

namespace LSKYDashboardDataCollector.SysAid
{
    public partial class JSONNewestTicket : System.Web.UI.Page
    {

        // Users can specify the number of tickets to load via the querystring (&count=7)

        // Default number of tickets to load if none is specified
        const int numTickets = 5;

        // Maximum tickets to display
        const int maxTickets = 25;

        private string removeDomainFromString(string input)
        {
            if (input.Contains(@"LSKYSD\"))
            {
                return input.Substring(7, input.Length - 7);
            }
            else
            {
                return input;
            }

        }

        private string timeSince(DateTime thisTime)
        {
            TimeSpan duration = DateTime.Now.Subtract(thisTime);
            String returnMe = string.Empty;

            if (duration.TotalMinutes < 1)
            {
                int totalSeconds = (int)Math.Round(duration.TotalSeconds, 0);
                if (totalSeconds == 1)
                {
                    returnMe = totalSeconds + " second ago";
                }
                else
                {
                    returnMe = totalSeconds + " seconds ago";
                }
            }
            else if (duration.TotalHours < 1)
            {
                int totalMinutes = (int)Math.Round(duration.TotalMinutes, 0);

                if (totalMinutes == 1)
                {
                    returnMe = totalMinutes + " minute ago";
                }
                else
                {
                    returnMe = totalMinutes + " minutes ago";
                }
            }
            else if (duration.TotalDays < 1)
            {
                int numHours = (int)Math.Round(duration.TotalHours, 0);

                if (numHours == 1)
                {
                    returnMe = numHours + " hour ago";
                }
                else
                {
                    returnMe = numHours + " hours ago";
                }
            }
            else
            {
                int numDays = (int)Math.Round(duration.TotalDays, 0);

                if (numDays == 1)
                {
                    returnMe = numDays + " day ago";
                }
                else
                {
                    returnMe = numDays + " days ago";
                }
            }

            return returnMe;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int loadThisManyTickets;
            if (!string.IsNullOrEmpty(Request.QueryString["count"]))
            {
                if (!int.TryParse(Request.QueryString["count"], out loadThisManyTickets))
                {
                    loadThisManyTickets = numTickets;
                }
            }
            else
            {
                loadThisManyTickets = numTickets;
            }

            if (loadThisManyTickets > maxTickets)
            {
                loadThisManyTickets = maxTickets;
            }

            List<ServiceRequest> allTickets = new List<ServiceRequest>();

            using (SqlConnection connection = new SqlConnection(SysAidCommon.dbConnectionString))
            {
                allTickets = ServiceRequest.loadNewestOpenServiceRequests(connection, loadThisManyTickets);
            }


            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n");
            Response.Write("\"Total\" : " + allTickets.Count + ",\n");
            Response.Write("\"Tickets\": [\n");

            for (int x = 0; x < allTickets.Count; x++)
            {
                Response.Write("{");
                Response.Write("\"location\" : \"" + allTickets[x].location + "\",");
                Response.Write("\"title\" : \"" + CommonFunctions.escapeCharacters(allTickets[x].title) + "\",");
                Response.Write("\"priority\" : \"" + allTickets[x].priority + "\",");
                Response.Write("\"inserted\" : \"" + allTickets[x].timeInserted + "\",");
                Response.Write("\"requested_by\" : \"" + removeDomainFromString(allTickets[x].requestedBy) + "\",");
                Response.Write("\"timesince\" : \"" + timeSince(allTickets[x].timeInserted) + "\"");
                Response.Write("}");

                if (!(x + 1 >= allTickets.Count))
                {
                    Response.Write(",");
                }

                Response.Write("\n");

            }

            Response.Write("]\n");
            Response.Write("}\n");
            Response.End();

        }
    }
}