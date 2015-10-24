using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.Jira
{
    public class JiraIssue
    {
        public int id { get; set; }
        public int IssueNumber { get; set; }
        public int projectID { get; set; }
        public string Reporter { get; set; }
        public string Assignee { get; set; }
        public string Creator { get; set; }
        public int IssueTypeID { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime DateResolved { get; set; }
        public DateTime DateDue { get; set; }
        public int Priority { get; set; }
        public string ProjectKey { get; set; }
        public string IssueKey { get; set; }
        public string ProjectName { get; set; }
        public string Facility { get; set; }

        public bool IsClosed
        {
            get {
                return DateResolved == DateTime.MinValue;
            }
        }
    }
}