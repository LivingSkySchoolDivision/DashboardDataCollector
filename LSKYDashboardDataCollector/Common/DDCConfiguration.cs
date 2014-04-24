using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.Common
{
    public static class DDCConfiguration
    {
        public static string GetiBossURL()
        {
            return System.Configuration.ConfigurationManager.AppSettings["iboss_url"].ToString();
        }

        public static string GetiBossUsername()
        {
            return System.Configuration.ConfigurationManager.AppSettings["iboss_username"].ToString();
        }

        public static string GetiBossPassword()
        {
            return System.Configuration.ConfigurationManager.AppSettings["iboss_password"].ToString();
        }

    }
}