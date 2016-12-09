using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LSKYDashboardDataCollector.Common;

namespace LSKYDashboardDataCollector.Versatrans
{
    public partial class UpcomingBusInspections : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VersaTransEmployeeRepository vtemployeeRepo = new VersaTransEmployeeRepository();

            List<VersaTransEmployee> employees = vtemployeeRepo.GetAllActive();


            Dictionary<VersatransCertification, VersaTransEmployee> allBusInspections = new Dictionary<VersatransCertification, VersaTransEmployee>();

            foreach (VersaTransEmployee employee in employees)
            {
                // If the employee doesn't have any vehicles associated with them, then the certification is meaningless
                if (employee.Vehicles.Count > 0)
                {
                    foreach (VersatransCertification cert in employee.Certifications)
                    {
                        if (cert.CertificationType == "bus inspection")
                        {
                            allBusInspections.Add(cert, employee);
                        }
                    }
                }
            }

            // Find all certifications expiring on or before the LAST DAY of the current month

            Dictionary<VersatransCertification, VersaTransEmployee> inspectionsDueThisMonth = new Dictionary<VersatransCertification, VersaTransEmployee>();
            Dictionary<VersatransCertification, VersaTransEmployee> inspectionsDueNextMonth = new Dictionary<VersatransCertification, VersaTransEmployee>();
            Dictionary<VersatransCertification, VersaTransEmployee> overdueInspections = new Dictionary<VersatransCertification, VersaTransEmployee>();

            DateTime startOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endOfThisMonth = new DateTime(startOfThisMonth.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            DateTime startOfNextMonth = endOfThisMonth.AddDays(1);
            DateTime endOfNextMonth = new DateTime(startOfNextMonth.Year, startOfNextMonth.Month, DateTime.DaysInMonth(startOfNextMonth.Year, startOfNextMonth.Month));

            foreach (VersatransCertification cert in allBusInspections.Keys)
            {
                // Overdue
                if (
                    (cert.Expires <= endOfThisMonth.AddMonths(-1))
                    )
                {
                    overdueInspections.Add(cert, allBusInspections[cert]);
                }

                // Current month
                if (
                    (cert.Expires >= startOfThisMonth) &&
                    (cert.Expires <= endOfThisMonth)
                    )
                {
                    inspectionsDueThisMonth.Add(cert, allBusInspections[cert]);
                }

                // Next month
                if (
                    (cert.Expires >= startOfNextMonth) &&
                    (cert.Expires <= endOfNextMonth)
                    )
                {
                    inspectionsDueNextMonth.Add(cert, allBusInspections[cert]);
                }


            }

            // Now go through and remove any inspections that have no valid vehicles associated with them
            foreach (KeyValuePair<VersatransCertification, VersaTransEmployee> kvp in inspectionsDueThisMonth.Where(i => i.Value.Vehicles.Count <= 0))
            {
                inspectionsDueThisMonth.Remove(kvp.Key);
            }

            foreach (KeyValuePair<VersatransCertification, VersaTransEmployee> kvp in inspectionsDueNextMonth.Where(i => i.Value.Vehicles.Count <= 0))
            {
                inspectionsDueThisMonth.Remove(kvp.Key);
            }

            foreach (KeyValuePair<VersatransCertification, VersaTransEmployee> kvp in overdueInspections.Where(i => i.Value.Vehicles.Count <= 0))
            {
                inspectionsDueThisMonth.Remove(kvp.Key);
            }


            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n");
            Response.Write("\"TotalOverdue\" : " + overdueInspections.Count + ",\n");
            Response.Write("\"Overdue\": [\n");

            int displaycount = 0;
            foreach (VersatransCertification cert in overdueInspections.Keys)
            {
                VersaTransEmployee driver = overdueInspections[cert];
                foreach (VersaTransVehicle vehicle in driver.Vehicles)
                {
                    Response.Write("\n{");
                    Response.Write("\"Vehicle\" : \"" + vehicle.VehicleNumber + "\",");
                    Response.Write("\"Driver\" : \"" + driver.DisplayName + "\",");
                    Response.Write("\"Expires\" : \"" + cert.Expires.ToShortDateString() + "\",");
                    Response.Write("\"Completed\" : \"" + cert.Completed.ToShortDateString() + "\"");
                    Response.Write("}");

                    if (!(displaycount + 1 >= overdueInspections.Count))
                    {
                        Response.Write(",");
                    }
                    displaycount++;
                }
            }

            Response.Write("],\n");
            Response.Write("\"ThisMonthName\": \"" + Helpers.GetMonthName(startOfThisMonth.Month) + "\",\n");
            Response.Write("\"TotalThisMonth\" : " + inspectionsDueThisMonth.Count + ",\n");
            Response.Write("\"ThisMonth\": [\n");
            displaycount = 0;

            foreach (VersatransCertification cert in inspectionsDueThisMonth.Keys)
            {
                VersaTransEmployee driver = inspectionsDueThisMonth[cert];
                foreach (VersaTransVehicle vehicle in driver.Vehicles)
                {
                    Response.Write("\n{");
                    Response.Write("\"Vehicle\" : \"" + vehicle.VehicleNumber + "\",");
                    Response.Write("\"Driver\" : \"" + driver.DisplayName + "\",");
                    Response.Write("\"Expires\" : \"" + cert.Expires.ToShortDateString() + "\",");
                    Response.Write("\"Completed\" : \"" + cert.Completed.ToShortDateString() + "\"");
                    Response.Write("}");

                    if (!(displaycount + 1 >= inspectionsDueThisMonth.Count))
                    {
                        Response.Write(",");
                    }
                    displaycount++;
                }

            }

            Response.Write("],\n");
            Response.Write("\"NextMonthName\": \"" + Helpers.GetMonthName(startOfNextMonth.Month) + "\",\n");
            Response.Write("\"TotalNextMonth\" : " + inspectionsDueNextMonth.Count + ",\n");
            Response.Write("\"NextMonth\": [\n");
            displaycount = 0;

            foreach (VersatransCertification cert in inspectionsDueNextMonth.Keys)
            {
                
                VersaTransEmployee driver = inspectionsDueNextMonth[cert];
                foreach (VersaTransVehicle vehicle in driver.Vehicles)
                {
                    Response.Write("\n{");
                    Response.Write("\"Vehicle\" : \"" + vehicle.VehicleNumber + "\",");
                    Response.Write("\"Driver\" : \"" + driver.DisplayName + "\",");
                    Response.Write("\"Expires\" : \"" + cert.Expires.ToShortDateString() + "\",");
                    Response.Write("\"Completed\" : \"" + cert.Completed.ToShortDateString() + "\"");
                    Response.Write("}");

                    if (!(displaycount + 1 >= inspectionsDueNextMonth.Count))
                    {
                        Response.Write(",");
                    }
                    displaycount++;
                }

            }


            Response.Write("]\n");

            Response.Write("}\n");
            Response.End();
            
        }
    }
}