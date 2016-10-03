﻿using System;
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
                foreach (VersatransCertification cert in employee.Certifications)
                {
                    if (cert.CertificationType == "bus inspection")
                    {
                        allBusInspections.Add(cert, employee);
                    }
                }
            }

            // Find all certifications expiring on or before the LAST DAY of the current month

            Dictionary<VersatransCertification, VersaTransEmployee> inspectionsDueThisMonth = new Dictionary<VersatransCertification, VersaTransEmployee>();
            Dictionary<VersatransCertification, VersaTransEmployee> overdueInspections = new Dictionary<VersatransCertification, VersaTransEmployee>();

            DateTime startOfThisMonth= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

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

            }

            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n");
            //Response.Write("\"Total\" : " + .Count + ",\n");
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
            Response.Write("]\n");

            Response.Write("}\n");
            Response.End();
            
        }
    }
}