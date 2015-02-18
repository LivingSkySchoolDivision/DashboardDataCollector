using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.SysAid
{
    public class CategoryEntry : IComparable
    {
        public string category { get; set; }
        public int count { get; set; }

        public CategoryEntry(string name, int count)
        {
            this.category = name;
            this.count = count;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            CategoryEntry obj2 = obj as CategoryEntry;

            if (obj2 != null)
            {
                //return this.count.CompareTo(obj2.count);
                return obj2.count.CompareTo(this.count);
            }
            else
            {
                throw new ArgumentException("Object is not a CategoryEntry");
            }
        }
    }
}