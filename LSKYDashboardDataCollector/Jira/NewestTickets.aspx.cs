using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LSKYDashboardDataCollector.Common;

namespace LSKYDashboardDataCollector.Jira
{
    public partial class NewestTickets : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            JiraIssueRepository repository = new JiraIssueRepository();
            List<JiraIssue> newestTickets = repository.GetRecent(10);
            
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n");
            Response.Write("\"Total\" : " + newestTickets.Count + ",\n");
            Response.Write("\"IncludedProjects\" : \"" + Settings.JiraProjectKeysToLoad.ToCommaSeperatedListWithQuotes() +  "\",\n");
            
            Response.Write("\"Tickets\": [\n");

            for (int x = 0; x < newestTickets.Count; x++)
            {
                Response.Write("{");
                Response.Write("\"location\" : \"" + newestTickets[x].Facility + "\",");
                Response.Write("\"title\" : \"" + CommonFunctions.escapeCharacters(newestTickets[x].Summary) + "\",");
                Response.Write("\"inserted\" : \"" + newestTickets[x].DateCreated + "\",");
                Response.Write("\"requested_by\" : \"" + newestTickets[x].Reporter + "\",");
                Response.Write("\"timesince\" : \"" + Helpers.TimeSince(newestTickets[x].DateCreated) + "\"");
                Response.Write("}");

                if (!(x + 1 >= newestTickets.Count))
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