using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.Jira
{
    public class TicketsByFacilityRecord
    {
        public string Facility { get; set; }
        public List<JiraIssue> OpenTickets { get; set; }
        public List<JiraIssue> AllTicketsLast30Days { get; set; }
    }

    public partial class TicketsByFacility : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            JiraIssueRepository repository = new JiraIssueRepository();
            List<string> allFacilities = repository.GetAllFacilities();
            List<JiraIssue> allOpenTickets = repository.GetAllUnresolved();
            List<JiraIssue> allTicketsLast30 = repository.GetIssuesCreatedDuring(DateTime.Now.AddDays(-30), DateTime.Now);
            
            List<TicketsByFacilityRecord> ticketsByFacility = new List<TicketsByFacilityRecord>();
            
            // Set up dictionaries 
            foreach (string facility in allFacilities)
            {
                ticketsByFacility.Add(new TicketsByFacilityRecord()
                {
                    Facility = facility,
                    AllTicketsLast30Days = allTicketsLast30.Where(i => i.Facility == facility).ToList(),
                    OpenTickets = allOpenTickets.Where(i => i.Facility == facility).ToList()
                });
            }
            // Add any that have no facility set
            ticketsByFacility.Add(new TicketsByFacilityRecord()
            {
                Facility = "No facility set",
                AllTicketsLast30Days = allTicketsLast30.Where(i => string.IsNullOrEmpty(i.Facility)).ToList(),
                OpenTickets = allOpenTickets.Where(i => string.IsNullOrEmpty(i.Facility)).ToList()
            });
            
            
            // Sort by facility
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";

            Response.Write("{\n \"TotalUnresolved\" : " + allOpenTickets.Count + ",\n");
            Response.Write("\"Facilities\": [\n");

            int facilityCounter = 0;
            foreach (TicketsByFacilityRecord facilityWithTickets in ticketsByFacility)
            {
                double TicketsPerDayLast7 = JiraStatistics.TicketsPerDay(facilityWithTickets.AllTicketsLast30Days.Where(i => i.DateCreated <= DateTime.Now && i.DateCreated >= DateTime.Today.AddDays(-7)).ToList(), DateTime.Now, DateTime.Now.AddDays(-7));
                double TicketsPerDayPreviousLast7 = JiraStatistics.TicketsPerDay(facilityWithTickets.AllTicketsLast30Days.Where(i => i.DateCreated <= DateTime.Now.AddDays(-7) && i.DateCreated >= DateTime.Today.AddDays(-14)).ToList(), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(-14));
                string trend = string.Empty;
                if (TicketsPerDayPreviousLast7 > TicketsPerDayLast7)
                {
                    trend = "Down";
                }
                else if (TicketsPerDayPreviousLast7 == TicketsPerDayLast7)
                {
                    trend = "Same";
                }
                else
                {
                    trend = "Up";
                }

                Response.Write("{ " +
                               "\"Facility\" : \"" + facilityWithTickets.Facility + "\", " +
                               "\"Unresolved\" : " + facilityWithTickets.OpenTickets.Count + ", " +
                               "\"CreatedLast7Days\" : " + facilityWithTickets.AllTicketsLast30Days.Count(i => i.DateCreated <= DateTime.Now && i.DateCreated >= DateTime.Today.AddDays(-7)) + ", " +
                               "\"CreatedPreviousLast7Days\" : " + facilityWithTickets.AllTicketsLast30Days.Count(i => i.DateCreated <= DateTime.Now.AddDays(-7) && i.DateCreated >= DateTime.Today.AddDays(-14)) + ", " +
                               "\"TicketsPerDayLast7Days\" : " + TicketsPerDayLast7  + ", " +
                               "\"TicketsPerDayPreviousLast7Days\" : " + TicketsPerDayPreviousLast7  + ", " +
                               "\"Trend\" : \"" + trend +"\"" +
                               "}");
                

                facilityCounter++;
                if (facilityCounter < ticketsByFacility.Count)
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