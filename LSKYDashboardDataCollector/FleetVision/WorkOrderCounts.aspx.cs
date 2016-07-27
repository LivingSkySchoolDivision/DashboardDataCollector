using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.FleetVision
{
    public partial class WorkOrderCounts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<FleetVisionWorkOrder> workOrders = new List<FleetVisionWorkOrder>();
            FleetVisionWorkOrderRepository repository = new FleetVisionWorkOrderRepository();
            workOrders = repository.GetAll();

            List<FleetVisionWorkOrder> wo_CreatedToday = repository.GetWorkOrdersCreatedDuring(DateTime.Today, DateTime.Now);
            List<FleetVisionWorkOrder> wo_CreatedYesterday = repository.GetWorkOrdersCreatedDuring(DateTime.Today.AddDays(-1), DateTime.Today);
            List<FleetVisionWorkOrder> wo_CreatedLast7Days = repository.GetWorkOrdersCreatedDuring(DateTime.Today.AddDays(-7), DateTime.Now);
            List<FleetVisionWorkOrder> wo_CreatedLast30Days = repository.GetWorkOrdersCreatedDuring(DateTime.Today.AddDays(-30), DateTime.Now);

            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n");

            Response.Write("\"WorkOrders\": {\n");
            Response.Write("\"Total\": " + workOrders.Count + ",\n");
            Response.Write("\"Open\": " + workOrders.Count(wo => !wo.IsClosed) + ",\n");
            Response.Write("\"Closed\": " + workOrders.Count(wo => wo.IsClosed) + ",\n");

            Response.Write("\"CreatedToday\": " + wo_CreatedToday.Count() + ",\n");
            Response.Write("\"CreatedYesterday\": " + wo_CreatedYesterday.Count() + ",\n");
            Response.Write("\"CreatedLast7Days\": " + wo_CreatedLast7Days.Count() + ",\n");
            Response.Write("\"CreatedLast30Days\": " + wo_CreatedLast30Days.Count() + "\n");


            Response.Write("}\n");
            
            Response.Write("}\n");
            Response.End();


        }
    }
}