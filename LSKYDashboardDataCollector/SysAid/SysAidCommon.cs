using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.SysAid
{
    public static class SysAidCommon
    {
        public static String dbConnectionString = ConfigurationManager.ConnectionStrings["SysAidDatabase"].ConnectionString;
    }
}