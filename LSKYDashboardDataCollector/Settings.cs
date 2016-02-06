using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector
{
    public static class Settings
    {
        public static string DBConnectionString_Jira
        {
            get { return ConfigurationManager.ConnectionStrings["Jira"].ConnectionString; }
        }

        public static List<string> JiraProjectKeysToLoad
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["jira_service_desk_project_keys"].ToString().Split(';').ToList();
            }
        }


        public static string SharePointUsername
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["sharepoint_username"].ToString();
            }
        }

        public static string SharePointPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["sharepoint_password"].ToString();
            }
        }

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

        public static string GetCiscoWirelessLogPath()
        {
            return System.Configuration.ConfigurationManager.AppSettings["cisco_wireless_path"].ToString();
        }
    }
}