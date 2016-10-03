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

    }
}