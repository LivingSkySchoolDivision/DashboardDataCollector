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
    public partial class JSONOpenTicketsByCategory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            List<ServiceRequest> allTickets = new List<ServiceRequest>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SysAidDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allTickets = ServiceRequest.loadOpenServiceRequests(connection);
            }
            
            SortedList<string, int> categories_1 = new SortedList<string, int>();
            SortedList<string, int> categories_2 = new SortedList<string, int>();

            /* Get a list of categories and their ticket counts */
            foreach (ServiceRequest ticket in allTickets)
            {
                if (!string.IsNullOrEmpty(ticket.category_1))
                {
                    if (!categories_1.ContainsKey(ticket.category_1))
                    {
                        categories_1.Add(ticket.category_1, 0);
                    }

                    categories_1[ticket.category_1]++;
                }


                if (!string.IsNullOrEmpty(ticket.category_2))
                {
                    if (!categories_2.ContainsKey(ticket.category_2))
                    {
                        categories_2.Add(ticket.category_2, 1);
                    }

                    categories_2[ticket.category_2]++;
                }


            }

            /* Put the counts in a structure so that I can sort it by count instead of alphabetically */

            List<CategoryEntry> categories_FirstLevel = new List<CategoryEntry>();
            foreach (KeyValuePair<string, int> category in categories_1)
            {
                categories_FirstLevel.Add(new CategoryEntry(category.Key, category.Value));
            }
            categories_FirstLevel.Sort();

            List<CategoryEntry> categories_SecondLevel = new List<CategoryEntry>();
            foreach (KeyValuePair<string, int> category in categories_2)
            {
                categories_SecondLevel.Add(new CategoryEntry(category.Key, category.Value));
            }
            categories_SecondLevel.Sort();




            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n \"Total\" : " + allTickets.Count + ",\n");
            Response.Write("\"Categories\": [");
            Response.Write("{\n");
            Response.Write("\"FirstLevel\": [\n");
            for (int x = 0; x < categories_FirstLevel.Count; x++)
            {
                Response.Write("{ \"name\": \"" + categories_FirstLevel[x].category + "\", \"count\" : " + categories_FirstLevel[x].count + " }");

                if (!(x + 1 >= categories_FirstLevel.Count))
                {
                    Response.Write(",");
                }
                Response.Write("\n");

            }

            Response.Write("]\n");
            Response.Write("},\n");

            Response.Write("{\n");
            Response.Write("\"SecondLevel\": [\n");
            for (int x = 0; x < categories_SecondLevel.Count; x++)
            {
                Response.Write("{ \"name\": \"" + categories_SecondLevel[x].category + "\", \"count\" : " + categories_SecondLevel[x].count + " }");

                if (!(x + 1 >= categories_SecondLevel.Count))
                {
                    Response.Write(",");
                }
                Response.Write("\n");

            }

            Response.Write("]\n");
            Response.Write("}\n");


            Response.Write("]\n");
            Response.Write("}\n");



            Response.End();
        }
    }
}