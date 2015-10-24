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
    }
}