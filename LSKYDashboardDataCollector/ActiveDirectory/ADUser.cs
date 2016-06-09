using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.ActiveDirectory
{
    public class ADUser
    {
        public string GivenName { get; set; }
        public string SN { get; set; }
        public string sAMAccountName { get; set; }
        public string comment { get; set; }
        public string description { get; set; }
        public bool IsEnabled { get; set; }
        public string LastLogon { get; set; }
        public string DateCreated { get; set; }
        public string DistinguishedName { get; set; }
        public string Mail { get; set; }

        public override string ToString()
        {
            return " {ADUSER " +
                   "givenName: " + this.GivenName + ", " +
                   "sn: " + this.SN + ", " +
                   "sAMAccountName: " + this.sAMAccountName + ", " +
                   "Enabled: " + this.IsEnabled + ", " +
                   "LastLogon: " + this.LastLogon + ", " +
                   "Mail: " + this.Mail + ", " +
                   "DN: " + this.DistinguishedName + " " +
                   "} ";
        }
    }
}