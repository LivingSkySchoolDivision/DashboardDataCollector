using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.SysAid
{
    public partial class JSONOpenTicketsBySchool : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<ServiceRequest> allTickets = new List<ServiceRequest>();

            using (SqlConnection connection = new SqlConnection(SysAidCommon.dbConnectionString))
            {
                allTickets = ServiceRequest.loadOpenServiceRequests(connection);
            }

            SortedList<string, int> schools = new SortedList<string, int>();

            foreach (ServiceRequest ticket in allTickets)
            {
                if (!string.IsNullOrEmpty(ticket.location))
                {
                    if (!schools.ContainsKey(ticket.location))
                    {
                        schools.Add(ticket.location, 0);
                    }
                    schools[ticket.location]++;
                }
            }

            /* Put numbers into objects so we can sort them differently */
            List<SchoolEntry> schoolsSorted = new List<SchoolEntry>();
            foreach (KeyValuePair<string, int> school in schools)
            {
                schoolsSorted.Add(new SchoolEntry(school.Key, school.Value));
            }

            schoolsSorted.Sort();

            foreach (SchoolEntry school in schoolsSorted)
            {
                Response.Write("<BR>School: " + school.location + ", Count: " + school.count);
            }

            /* Create a JSON file */
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";

            Response.Write("{\n \"Total\" : " + allTickets.Count + ",\n");
            Response.Write("\"Schools\": [\n");

            for (int x = 0; x < schoolsSorted.Count(); x++)
            {
                Response.Write("{ \"location\" : \"" + schoolsSorted[x].location + "\", \"count\" : " + schoolsSorted[x].count + " }");
                if (!(x + 1 >= schoolsSorted.Count))
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