using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.Versatrans
{
    public class VersatransCertification
    {
        public int RecordID { get; set; }

        public string CertificationType { get; set; }
        public DateTime Completed { get; set; }
        public DateTime Expires { get; set; }


        public int EmployeeID { get; set; }

        public override string ToString()
        {
            return "{ ID: " + this.RecordID +" CertificationType: " + this.CertificationType +" Completed: " + this.Completed +" Expires: " + this.Expires +" EmployeeID: " + this.EmployeeID + " }";
        }
    }
}