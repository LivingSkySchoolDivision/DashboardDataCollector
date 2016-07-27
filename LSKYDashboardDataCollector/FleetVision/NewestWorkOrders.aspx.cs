using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LSKYDashboardDataCollector.Common;

namespace LSKYDashboardDataCollector.FleetVision
{
    public partial class NewestWorkOrders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<FleetVisionWorkOrder> workOrders = new List<FleetVisionWorkOrder>();

            FleetVisionWorkOrderRepository repository = new FleetVisionWorkOrderRepository();

            workOrders = repository.GetRecentIncomplete(10);

            foreach (FleetVisionWorkOrder wo in workOrders)
            {
                Response.Write("<BR>" + wo.ID + ": " + wo.Status + " - " + wo.WorkRequested);
            }


            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n");
            Response.Write("\"WorkOrders\": [\n");

            for (int x = 0; x < workOrders.Count; x++)
            {
                Response.Write("{");
                Response.Write("\"id\" : \"" + workOrders[x].ID + "\",");
                Response.Write("\"number\" : \"" + workOrders[x].WorkOrderNumber + "\",");
                Response.Write("\"createdby\" : \"" + workOrders[x].CreatedBy + "\",");
                Response.Write("\"status\" : \"" + workOrders[x].Status + "\",");
                Response.Write("\"timesince\" : \"" + Helpers.TimeSince(workOrders[x].DateCreated) + "\",");
                Response.Write("\"workrequested\" : \"" + CommonFunctions.escapeCharacters(workOrders[x].WorkRequested) + "\"");
                
                Response.Write("}");

                if (!(x + 1 >= workOrders.Count))
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