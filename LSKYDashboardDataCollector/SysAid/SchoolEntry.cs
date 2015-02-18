using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.SysAid
{
    public class SchoolEntry : IComparable
    {
        public string location { get; set; }
        public int count { get; set; }

        public SchoolEntry(string location, int count)
        {
            this.location = location;
            this.count = count;
        }
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            SchoolEntry obj2 = obj as SchoolEntry;

            if (obj2 != null)
            {
                return obj2.count.CompareTo(this.count);
            }
            else
            {
                throw new ArgumentException("Object is not a SchoolEntry");
            }
        }
    }
}