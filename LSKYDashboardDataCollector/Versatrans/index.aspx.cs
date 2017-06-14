using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.Versatrans
{
    public partial class index : System.Web.UI.Page
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

            Dictionary<string, List<VersatransCertification>> certificationsByMonth = new Dictionary<string, List<VersatransCertification>>();

            foreach (VersatransCertification cert in allBusInspections.Keys.OrderBy(x => x.Expires))
            {
                string dateKey = cert.Expires.Year + "-" + Helpers.GetMonthName(cert.Expires.Month);

                if (!certificationsByMonth.ContainsKey(dateKey))
                {
                    certificationsByMonth.Add(dateKey, new List<VersatransCertification>());
                }
                certificationsByMonth[dateKey].Add(cert);
            }

            Response.Write("<div style=\"font-family: Arial\">");

            // Display on page
            foreach (string dateKey in certificationsByMonth.Keys)
            {
                Response.Write("<h1 style=\"margin-bottom: 2; padding-bottom: 2;\">" + dateKey + "</h1>");

                Response.Write("<table border=1 style=\"width: 600px;\"><tr><td><b>Vehicle</b></td><td><b>Driver</b></td><td><b>Expires</b></td><td><b>Last Complete</b></td></tr>");
                foreach(VersatransCertification cert in certificationsByMonth[dateKey])
                {
                    if (allBusInspections.ContainsKey(cert))
                    {
                        VersaTransEmployee driver = allBusInspections[cert];
                        foreach (VersaTransVehicle vehicle in driver.Vehicles)
                        {
                            Response.Write("<tr><td>" + vehicle.VehicleNumber + "</td><td>" + driver.DisplayName + "</td><td>" + cert.Expires.ToShortDateString() + "</td><td>" + cert.Completed.ToShortDateString() + "</td></tr>");
                        }
                    }
                    
                }
                Response.Write("</table><br>");

                // Vehicle
                // Driver
                // Expires
                // Completed
            }
            Response.Write("</div>");




        }
    }
}