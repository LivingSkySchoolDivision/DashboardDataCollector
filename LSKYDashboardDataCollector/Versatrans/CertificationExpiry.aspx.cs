using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.Versatrans
{
    public partial class CertificationExpiry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VersaTransEmployeeRepository vtemployeeRepo = new VersaTransEmployeeRepository();

            List<VersaTransEmployee> employees = vtemployeeRepo.GetAllActive();

            foreach (VersaTransEmployee employee in employees)
            {
                Response.Write("<BR><b>" + employee + "</b>");
                Response.Write("<BR>&nbsp;&nbsp;<b>Vehicles</b>");
                foreach (VersaTransVehicle vehicle in employee.Vehicles)
                {
                    Response.Write("<BR>&nbsp;&nbsp;&nbsp; Vehicle: " + vehicle.VehicleNumber);
                }
                Response.Write("<BR>&nbsp;&nbsp;<b>Certifications</b>");
                foreach (VersatransCertification cert in employee.Certifications)
                {
                    Response.Write("<BR>&nbsp;&nbsp;&nbsp; Cert: " + cert.CertificationType + " Expires: " + cert.Expires);
                }
            }

        }
    }
}