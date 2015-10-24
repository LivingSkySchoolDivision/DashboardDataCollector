using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.Jira
{
    public partial class TicketCounts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            JiraIssueRepository repository = new JiraIssueRepository();
            List<JiraIssue> ticketsCreatedLast30Days = repository.GetIssuesCreatedDuring(DateTime.Now.AddDays(-30), DateTime.Now);
            List<JiraIssue> ticketsClosedLast30Days = repository.GetIssuesCreatedDuring(DateTime.Now.AddDays(-30), DateTime.Now);
            List<JiraIssue> allOpenTickets = repository.GetAllUnresolved();
            List<JiraIssue> allClosedTickets = repository.GetAllResolved();

            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n\"Stats\": {");

            Response.Write("\"ServiceRequests\": {\n");
            Response.Write("\"Total\": " + (int)((int)allOpenTickets.Count + (int)allClosedTickets.Count) + ",\n");
            Response.Write("\"Open\": " + allOpenTickets.Count + ",\n");
            Response.Write("\"Closed\": " + allClosedTickets.Count + ",\n");
            Response.Write("\"Recent\": {");
            {
                Response.Write("\"Today\": {\n");
                Response.Write("\"Created\": " + ticketsCreatedLast30Days.Count(i => i.DateCreated >= DateTime.Today && i.DateCreated < DateTime.Today.AddDays(1)) + ",\n");
                Response.Write("\"Closed\": " + ticketsClosedLast30Days.Count(i => i.DateResolved >= DateTime.Today && i.DateResolved < DateTime.Today.AddDays(1)) + "\n");
                Response.Write("},\n");

                Response.Write("\"Yesterday\": {\n");
                Response.Write("\"Created\": " + ticketsCreatedLast30Days.Count(i => i.DateCreated >= DateTime.Today.AddDays(-1) && i.DateCreated < DateTime.Today) + ",\n");
                Response.Write("\"Closed\": " + ticketsClosedLast30Days.Count(i => i.DateResolved >= DateTime.Today.AddDays(-1) && i.DateResolved < DateTime.Today) + "\n");
                Response.Write("},\n");

                Response.Write("\"Last7Days\": {\n");
                Response.Write("\"Created\": " + ticketsCreatedLast30Days.Count(i => i.DateCreated >= DateTime.Today.AddDays(-7) && i.DateCreated < DateTime.Today.AddDays(1)) + ",\n");
                Response.Write("\"Closed\": " + ticketsClosedLast30Days.Count(i => i.DateResolved >= DateTime.Today.AddDays(-7) && i.DateResolved < DateTime.Today.AddDays(1)) + "\n");
                Response.Write("},\n");

                Response.Write("\"Last30Days\": {\n");
                Response.Write("\"Created\": " + ticketsCreatedLast30Days.Count() + ",\n");
                Response.Write("\"Closed\": " + ticketsClosedLast30Days.Count() + "\n");
                Response.Write("}\n");
            }
            Response.Write("}\n");

            Response.Write("}\n");

            Response.Write("}\n");
            Response.Write("}\n");
            Response.End();
        }
    }
}