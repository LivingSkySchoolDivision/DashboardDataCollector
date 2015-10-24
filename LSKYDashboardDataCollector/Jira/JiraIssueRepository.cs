using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using LSKYDashboardDataCollector.Common;

namespace LSKYDashboardDataCollector.Jira
{
    public class JiraIssueRepository
    {
        private JiraIssue SQLDataReaderToJiraIssue(SqlDataReader dataReader)
        {
            return new JiraIssue()
            {
                id = Parsers.ParseInt(dataReader["ID"].ToString().Trim()),
                projectID = Parsers.ParseInt(dataReader["PROJECT"].ToString().Trim()),
                Reporter = dataReader["REPORTER"].ToString().Trim(),
                Assignee = dataReader["ASSIGNEE"].ToString().Trim(),
                Creator = dataReader["CREATOR"].ToString().Trim(),
                IssueTypeID = Parsers.ParseInt(dataReader["issuetype"].ToString().Trim()),
                Summary = dataReader["SUMMARY"].ToString().Trim(),
                Description = dataReader["DESCRIPTION"].ToString().Trim(),
                DateCreated = Parsers.ParseDate(dataReader["CREATED"].ToString().Trim()),
                DateUpdated = Parsers.ParseDate(dataReader["UPDATED"].ToString().Trim()),
                DateResolved = Parsers.ParseDate(dataReader["RESOLUTIONDATE"].ToString().Trim()),
                DateDue = Parsers.ParseDate(dataReader["DUEDATE"].ToString().Trim()),
                Priority = Parsers.ParseInt(dataReader["PRIORITY"].ToString().Trim()),
                IssueNumber = Parsers.ParseInt(dataReader["issuenum"].ToString().Trim()),
                ProjectKey = dataReader["pkey"].ToString().Trim(),
                ProjectName = dataReader["pname"].ToString().Trim(),
                IssueKey = dataReader["pkey"].ToString().Trim() + "-" + Parsers.ParseInt(dataReader["issuenum"].ToString().Trim()),
                Facility = dataReader["Facility"].ToString().Trim()
            };
        }
        
        /// <summary>
        /// Gets the n most recent issues 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<JiraIssue> GetRecent(int count)
        {
            List<JiraIssue> returnMe = new List<JiraIssue>();

            using (SqlConnection connection = new SqlConnection(Settings.DBConnectionString_Jira))
            {
                SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT TOP " + count + " jiraissue.ID, jiraissue.issuenum, jiraissue.PROJECT, jiraissue.REPORTER, jiraissue.ASSIGNEE, jiraissue.CREATOR, jiraissue.issuetype, jiraissue.SUMMARY, jiraissue.ENVIRONMENT, jiraissue.DESCRIPTION, jiraissue.PRIORITY, jiraissue.RESOLUTION, jiraissue.issuestatus, jiraissue.CREATED, jiraissue.DUEDATE, jiraissue.UPDATED, jiraissue.RESOLUTIONDATE, jiraissue.VOTES, jiraissue.WATCHES, jiraissue.TIMEORIGINALESTIMATE, jiraissue.TIMEESTIMATE, jiraissue.TIMESPENT, jiraissue.WORKFLOW_ID, jiraissue.SECURITY, jiraissue.FIXFOR, jiraissue.COMPONENT, project.pname, project.pkey, derivedtbl_1.customvalue AS Facility FROM (SELECT customfieldvalue.ISSUE, customfieldoption.customvalue FROM customfieldoption RIGHT OUTER JOIN customfieldvalue ON customfieldoption.ID = customfieldvalue.STRINGVALUE WHERE (customfieldvalue.CUSTOMFIELD = 10501)) AS derivedtbl_1 RIGHT OUTER JOIN jiraissue ON derivedtbl_1.ISSUE = jiraissue.ID LEFT OUTER JOIN project ON jiraissue.PROJECT = project.ID" +
                                  " WHERE (RESOLUTIONDATE IS NULL) AND (project.pkey IN (" + Settings.JiraProjectKeysToLoad.ToCommaSeperatedListWithQuotes() + ")) ORDER BY jiraissue.CREATED DESC;"
                };
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();
                
                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        JiraIssue loadedSchool = SQLDataReaderToJiraIssue(dbDataReader);
                        if (loadedSchool != null)
                        {
                            returnMe.Add(loadedSchool);
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }

            return returnMe;
        }

        public List<JiraIssue> GetIssuesCreatedDuring(DateTime timeFrom, DateTime timeTo)
        {
            List<JiraIssue> returnMe = new List<JiraIssue>();

            using (SqlConnection connection = new SqlConnection(Settings.DBConnectionString_Jira))
            {
                SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT jiraissue.ID, jiraissue.issuenum, jiraissue.PROJECT, jiraissue.REPORTER, jiraissue.ASSIGNEE, jiraissue.CREATOR, jiraissue.issuetype, jiraissue.SUMMARY, jiraissue.ENVIRONMENT, jiraissue.DESCRIPTION, jiraissue.PRIORITY, jiraissue.RESOLUTION, jiraissue.issuestatus, jiraissue.CREATED, jiraissue.DUEDATE, jiraissue.UPDATED, jiraissue.RESOLUTIONDATE, jiraissue.VOTES, jiraissue.WATCHES, jiraissue.TIMEORIGINALESTIMATE, jiraissue.TIMEESTIMATE, jiraissue.TIMESPENT, jiraissue.WORKFLOW_ID, jiraissue.SECURITY, jiraissue.FIXFOR, jiraissue.COMPONENT, project.pname, project.pkey, derivedtbl_1.customvalue AS Facility FROM (SELECT customfieldvalue.ISSUE, customfieldoption.customvalue FROM customfieldoption RIGHT OUTER JOIN customfieldvalue ON customfieldoption.ID = customfieldvalue.STRINGVALUE WHERE (customfieldvalue.CUSTOMFIELD = 10501)) AS derivedtbl_1 RIGHT OUTER JOIN jiraissue ON derivedtbl_1.ISSUE = jiraissue.ID LEFT OUTER JOIN project ON jiraissue.PROJECT = project.ID" +
                                  " WHERE (jiraissue.CREATED >= @DATEFROM) AND (jiraissue.CREATED <= @DATETO) AND (project.pkey IN (" + Settings.JiraProjectKeysToLoad.ToCommaSeperatedListWithQuotes() + ")) ORDER BY jiraissue.CREATED DESC;"
                };
                sqlCommand.Parameters.AddWithValue("DATEFROM", timeFrom);
                sqlCommand.Parameters.AddWithValue("DATETO", timeTo);
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        JiraIssue loadedSchool = SQLDataReaderToJiraIssue(dbDataReader);
                        if (loadedSchool != null)
                        {
                            returnMe.Add(loadedSchool);
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }

            return returnMe;
        }

        public List<JiraIssue> GetIssuesClosedDuring(DateTime timeFrom, DateTime timeTo)
        {
            List<JiraIssue> returnMe = new List<JiraIssue>();

            using (SqlConnection connection = new SqlConnection(Settings.DBConnectionString_Jira))
            {
                SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT jiraissue.ID, jiraissue.issuenum, jiraissue.PROJECT, jiraissue.REPORTER, jiraissue.ASSIGNEE, jiraissue.CREATOR, jiraissue.issuetype, jiraissue.SUMMARY, jiraissue.ENVIRONMENT, jiraissue.DESCRIPTION, jiraissue.PRIORITY, jiraissue.RESOLUTION, jiraissue.issuestatus, jiraissue.CREATED, jiraissue.DUEDATE, jiraissue.UPDATED, jiraissue.RESOLUTIONDATE, jiraissue.VOTES, jiraissue.WATCHES, jiraissue.TIMEORIGINALESTIMATE, jiraissue.TIMEESTIMATE, jiraissue.TIMESPENT, jiraissue.WORKFLOW_ID, jiraissue.SECURITY, jiraissue.FIXFOR, jiraissue.COMPONENT, project.pname, project.pkey, derivedtbl_1.customvalue AS Facility FROM (SELECT customfieldvalue.ISSUE, customfieldoption.customvalue FROM customfieldoption RIGHT OUTER JOIN customfieldvalue ON customfieldoption.ID = customfieldvalue.STRINGVALUE WHERE (customfieldvalue.CUSTOMFIELD = 10501)) AS derivedtbl_1 RIGHT OUTER JOIN jiraissue ON derivedtbl_1.ISSUE = jiraissue.ID LEFT OUTER JOIN project ON jiraissue.PROJECT = project.ID" +
                                  " WHERE (jiraissue.RESOLUTIONDATE >= @DATEFROM) AND (jiraissue.RESOLUTIONDATE <= @DATETO) AND (project.pkey IN (" + Settings.JiraProjectKeysToLoad.ToCommaSeperatedListWithQuotes() + ")) ORDER BY jiraissue.CREATED DESC;"
                };
                sqlCommand.Parameters.AddWithValue("DATEFROM", timeFrom);
                sqlCommand.Parameters.AddWithValue("DATETO", timeTo);
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        JiraIssue loadedSchool = SQLDataReaderToJiraIssue(dbDataReader);
                        if (loadedSchool != null)
                        {
                            returnMe.Add(loadedSchool);
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }

            return returnMe;
        }

        public List<JiraIssue> GetIssuesUpdatedDuring(DateTime timeFrom, DateTime timeTo)
        {
            List<JiraIssue> returnMe = new List<JiraIssue>();

            using (SqlConnection connection = new SqlConnection(Settings.DBConnectionString_Jira))
            {
                SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT jiraissue.ID, jiraissue.issuenum, jiraissue.PROJECT, jiraissue.REPORTER, jiraissue.ASSIGNEE, jiraissue.CREATOR, jiraissue.issuetype, jiraissue.SUMMARY, jiraissue.ENVIRONMENT, jiraissue.DESCRIPTION, jiraissue.PRIORITY, jiraissue.RESOLUTION, jiraissue.issuestatus, jiraissue.CREATED, jiraissue.DUEDATE, jiraissue.UPDATED, jiraissue.RESOLUTIONDATE, jiraissue.VOTES, jiraissue.WATCHES, jiraissue.TIMEORIGINALESTIMATE, jiraissue.TIMEESTIMATE, jiraissue.TIMESPENT, jiraissue.WORKFLOW_ID, jiraissue.SECURITY, jiraissue.FIXFOR, jiraissue.COMPONENT, project.pname, project.pkey, derivedtbl_1.customvalue AS Facility FROM (SELECT customfieldvalue.ISSUE, customfieldoption.customvalue FROM customfieldoption RIGHT OUTER JOIN customfieldvalue ON customfieldoption.ID = customfieldvalue.STRINGVALUE WHERE (customfieldvalue.CUSTOMFIELD = 10501)) AS derivedtbl_1 RIGHT OUTER JOIN jiraissue ON derivedtbl_1.ISSUE = jiraissue.ID LEFT OUTER JOIN project ON jiraissue.PROJECT = project.ID" +
                                  " WHERE (jiraissue.UPDATED >= @DATEFROM) AND (jiraissue.UPDATED <= @DATETO) AND (project.pkey IN (" + Settings.JiraProjectKeysToLoad.ToCommaSeperatedListWithQuotes() + ")) ORDER BY jiraissue.CREATED DESC;"
                };
                sqlCommand.Parameters.AddWithValue("DATEFROM", timeFrom);
                sqlCommand.Parameters.AddWithValue("DATETO", timeTo);
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        JiraIssue loadedSchool = SQLDataReaderToJiraIssue(dbDataReader);
                        if (loadedSchool != null)
                        {
                            returnMe.Add(loadedSchool);
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }

            return returnMe;
        }

        public List<JiraIssue> GetAllUnresolved()
        {
            List<JiraIssue> returnMe = new List<JiraIssue>();

            using (SqlConnection connection = new SqlConnection(Settings.DBConnectionString_Jira))
            {
                SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT jiraissue.ID, jiraissue.issuenum, jiraissue.PROJECT, jiraissue.REPORTER, jiraissue.ASSIGNEE, jiraissue.CREATOR, jiraissue.issuetype, jiraissue.SUMMARY, jiraissue.ENVIRONMENT, jiraissue.DESCRIPTION, jiraissue.PRIORITY, jiraissue.RESOLUTION, jiraissue.issuestatus, jiraissue.CREATED, jiraissue.DUEDATE, jiraissue.UPDATED, jiraissue.RESOLUTIONDATE, jiraissue.VOTES, jiraissue.WATCHES, jiraissue.TIMEORIGINALESTIMATE, jiraissue.TIMEESTIMATE, jiraissue.TIMESPENT, jiraissue.WORKFLOW_ID, jiraissue.SECURITY, jiraissue.FIXFOR, jiraissue.COMPONENT, project.pname, project.pkey, derivedtbl_1.customvalue AS Facility FROM (SELECT customfieldvalue.ISSUE, customfieldoption.customvalue FROM customfieldoption RIGHT OUTER JOIN customfieldvalue ON customfieldoption.ID = customfieldvalue.STRINGVALUE WHERE (customfieldvalue.CUSTOMFIELD = 10501)) AS derivedtbl_1 RIGHT OUTER JOIN jiraissue ON derivedtbl_1.ISSUE = jiraissue.ID LEFT OUTER JOIN project ON jiraissue.PROJECT = project.ID" +
                                  " WHERE (jiraissue.RESOLUTIONDATE IS NULL) AND (project.pkey IN (" + Settings.JiraProjectKeysToLoad.ToCommaSeperatedListWithQuotes() + ")) ORDER BY jiraissue.CREATED DESC;"
                };
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        JiraIssue loadedSchool = SQLDataReaderToJiraIssue(dbDataReader);
                        if (loadedSchool != null)
                        {
                            returnMe.Add(loadedSchool);
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }

            return returnMe;
        }

        public List<JiraIssue> GetAllResolved()
        {
            List<JiraIssue> returnMe = new List<JiraIssue>();

            using (SqlConnection connection = new SqlConnection(Settings.DBConnectionString_Jira))
            {
                SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT jiraissue.ID, jiraissue.issuenum, jiraissue.PROJECT, jiraissue.REPORTER, jiraissue.ASSIGNEE, jiraissue.CREATOR, jiraissue.issuetype, jiraissue.SUMMARY, jiraissue.ENVIRONMENT, jiraissue.DESCRIPTION, jiraissue.PRIORITY, jiraissue.RESOLUTION, jiraissue.issuestatus, jiraissue.CREATED, jiraissue.DUEDATE, jiraissue.UPDATED, jiraissue.RESOLUTIONDATE, jiraissue.VOTES, jiraissue.WATCHES, jiraissue.TIMEORIGINALESTIMATE, jiraissue.TIMEESTIMATE, jiraissue.TIMESPENT, jiraissue.WORKFLOW_ID, jiraissue.SECURITY, jiraissue.FIXFOR, jiraissue.COMPONENT, project.pname, project.pkey, derivedtbl_1.customvalue AS Facility FROM (SELECT customfieldvalue.ISSUE, customfieldoption.customvalue FROM customfieldoption RIGHT OUTER JOIN customfieldvalue ON customfieldoption.ID = customfieldvalue.STRINGVALUE WHERE (customfieldvalue.CUSTOMFIELD = 10501)) AS derivedtbl_1 RIGHT OUTER JOIN jiraissue ON derivedtbl_1.ISSUE = jiraissue.ID LEFT OUTER JOIN project ON jiraissue.PROJECT = project.ID" +
                                  " WHERE (jiraissue.RESOLUTIONDATE IS NOT NULL) AND (project.pkey IN (" + Settings.JiraProjectKeysToLoad.ToCommaSeperatedListWithQuotes() + ")) ORDER BY jiraissue.CREATED DESC;"
                };
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        JiraIssue loadedSchool = SQLDataReaderToJiraIssue(dbDataReader);
                        if (loadedSchool != null)
                        {
                            returnMe.Add(loadedSchool);
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }

            return returnMe;
        }

        public List<string> GetAllFacilities()
        {
            List<string> returnMe = new List<string>();

            using (SqlConnection connection = new SqlConnection(Settings.DBConnectionString_Jira))
            {
                SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT * FROM customfieldoption WHERE CUSTOMFIELD=10501;"
                };
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        string value = dbDataReader["customvalue"].ToString().Trim();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (!returnMe.Contains(value))
                            {
                                returnMe.Add(value);
                            }
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }

            return returnMe.OrderBy(r => r).ToList();
        } 
    }

}