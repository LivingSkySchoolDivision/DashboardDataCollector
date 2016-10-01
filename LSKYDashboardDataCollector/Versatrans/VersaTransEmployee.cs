using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.Versatrans
{
    public class VersaTransEmployee
    {
        public int RecordID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeNumber { get; set; }
        public bool IsActive { get; set; }

        public List<VersatransCertification> Certifications { get; set; }
        public List<VersaTransVehicle> Vehicles { get; set; }

        public VersaTransEmployee()
        {
            Certifications = new List<VersatransCertification>();
            Vehicles = new List<VersaTransVehicle>();
        }

        public override string ToString()
        {
            return "{ VersaTransEmployee: " + RecordID + " " + LastName  + ", " + FirstName + " Certifications: " + Certifications.Count + " Vehicles: " + Vehicles.Count + " }";
        }
    }
}