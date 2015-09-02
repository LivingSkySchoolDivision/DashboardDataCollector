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
    public partial class JSONTicketCounts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int openTickets = 0;
            int closedTickets = 0;
            int endUserCount = 0;
            int technicianCount = 0;
            int assetCount = 0;
            int onlineAssetCount = 0;

            int ticketsClosed_Today = 0;
            int ticketsClosed_Yesterday = 0;
            int ticketsClosed_Last7Days = 0;
            int ticketsClosed_Last30Days = 0;

            int ticketsCreated_Today = 0;
            int ticketsCreated_Yesterday = 0;
            int ticketsCreated_Last7Days = 0;
            int ticketsCreated_Last30Days = 0;

            using (SqlConnection connection = new SqlConnection(SysAidCommon.dbConnectionString))
            {
                openTickets = ServiceRequest.loadOpenRequestCount(connection);
                closedTickets = ServiceRequest.loadClosedRequestCount(connection);
                technicianCount = SysAidUser.loadTechnicianCount(connection);
                endUserCount = SysAidUser.loadEndUserCount(connection);
                assetCount = SysAidAsset.loadAssetCount(connection);
                onlineAssetCount = SysAidAsset.loadOnlineAssetCount(connection);

                ticketsCreated_Today = ServiceRequest.loadNumberOfNewTickets(connection, DateTime.Today, DateTime.Now);
                ticketsCreated_Yesterday = ServiceRequest.loadNumberOfNewTickets(connection, DateTime.Today.AddDays(-1), DateTime.Today);
                ticketsCreated_Last7Days = ServiceRequest.loadNumberOfNewTickets(connection, DateTime.Today.AddDays(-7), DateTime.Now);
                ticketsCreated_Last30Days = ServiceRequest.loadNumberOfNewTickets(connection, DateTime.Today.AddDays(-30), DateTime.Now);

                ticketsClosed_Today = ServiceRequest.loadNumberOfTicketCloses(connection, DateTime.Today, DateTime.Now);
                ticketsClosed_Yesterday = ServiceRequest.loadNumberOfTicketCloses(connection, DateTime.Today.AddDays(-1), DateTime.Today);
                ticketsClosed_Last7Days = ServiceRequest.loadNumberOfTicketCloses(connection, DateTime.Today.AddDays(-7), DateTime.Now);
                ticketsClosed_Last30Days = ServiceRequest.loadNumberOfTicketCloses(connection, DateTime.Today.AddDays(-30), DateTime.Now);
            }

            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n\"Stats\": {");

            Response.Write("\"ServiceRequests\": {\n");
            Response.Write("\"Total\": " + (openTickets + closedTickets) + ",\n");
            Response.Write("\"Open\": " + openTickets + ",\n");
            Response.Write("\"Closed\": " + closedTickets + ",\n");
            Response.Write("\"Recent\": {");
            {
                Response.Write("\"Today\": {\n");
                Response.Write("\"Created\": " + ticketsCreated_Today + ",\n");
                Response.Write("\"Closed\": " + ticketsClosed_Today + "\n");
                Response.Write("},\n");

                Response.Write("\"Yesterday\": {\n");
                Response.Write("\"Created\": " + ticketsCreated_Yesterday + ",\n");
                Response.Write("\"Closed\": " + ticketsClosed_Yesterday + "\n");
                Response.Write("},\n");

                Response.Write("\"Last7Days\": {\n");
                Response.Write("\"Created\": " + ticketsCreated_Last7Days + ",\n");
                Response.Write("\"Closed\": " + ticketsClosed_Last7Days + "\n");
                Response.Write("},\n");

                Response.Write("\"Last30Days\": {\n");
                Response.Write("\"Created\": " + ticketsCreated_Last30Days + ",\n");
                Response.Write("\"Closed\": " + ticketsClosed_Last30Days + "\n");
                Response.Write("}\n");
            }
            Response.Write("}\n");

            Response.Write("},\n");

            Response.Write("\"UserAccounts\": {\n");
            Response.Write("\"EndUsers\": " + endUserCount + ",\n");
            Response.Write("\"Technicians\": " + technicianCount + "\n");
            Response.Write("},\n");

            Response.Write("\"Assets\": {\n");
            Response.Write("\"AssetCount\": " + assetCount + ",\n");
            Response.Write("\"Online\": " + onlineAssetCount + "\n");
            Response.Write("}\n");

            Response.Write("}\n");
            Response.Write("}\n");
            Response.End();
        }
    }
}