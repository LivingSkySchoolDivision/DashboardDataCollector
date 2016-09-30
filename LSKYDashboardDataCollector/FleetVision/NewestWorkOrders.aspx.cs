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

            workOrders = repository.GetRecentIncomplete(50).Where(wo => wo.InDateTime <= DateTime.Today.AddDays(1).AddMinutes(-1)).OrderBy(wo => wo.PrioritySortOrder).ThenBy(wo => wo.VehicleNumberForSorting).ToList();
            
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n");
            Response.Write("\"Total\" : " + workOrders.Count + ",\n");
            Response.Write("\"WorkOrders\": [\n");

            for (int x = 0; x < workOrders.Count; x++)
            {
                if (workOrders[x].Vehicle != null) { }
                Response.Write("{");
                Response.Write("\"RecordID\" : \"" + workOrders[x].RecordID + "\",");
                Response.Write("\"number\" : \"" + workOrders[x].WorkOrderNumber + "\",");
                Response.Write("\"createdby\" : \"" + workOrders[x].CreatedBy + "\",");
                Response.Write("\"status\" : \"" + workOrders[x].Status + "\",");
                Response.Write("\"timesince\" : \"" + Helpers.TimeSince(workOrders[x].DateCreated) + "\",");
                Response.Write("\"priority\" : \"" + CommonFunctions.escapeCharacters(workOrders[x].Priority) + "\",");
                Response.Write("\"status\" : \"" + CommonFunctions.escapeCharacters(workOrders[x].Status) + "\",");
                Response.Write("\"workrequested\" : \"" + CommonFunctions.escapeCharacters(workOrders[x].WorkRequested.Replace("\r\n", ", ")) + "\",");

                string vehicle = "NONE";
                string plate = string.Empty;
                string vin = string.Empty;

                if (workOrders[x].Vehicle != null)
                {
                    vehicle = workOrders[x].Vehicle.VehicleNumber;
                    plate = workOrders[x].Vehicle.Plate;
                    vin = workOrders[x].Vehicle.VIN;
                }
                Response.Write("\"vehicleID\" : \"" + CommonFunctions.escapeCharacters(workOrders[x].VehicleRecordID.ToString()) + "\",");
                Response.Write("\"vehicle\" : \"" + CommonFunctions.escapeCharacters(vehicle) + "\",");
                Response.Write("\"licenseplate\" : \"" + CommonFunctions.escapeCharacters(plate) + "\",");
                Response.Write("\"vin\" : \"" + CommonFunctions.escapeCharacters(vin) + "\"");
                
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