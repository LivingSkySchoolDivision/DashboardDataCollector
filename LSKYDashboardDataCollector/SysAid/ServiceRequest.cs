using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using LSKYDashboardDataCollector.Common;

namespace LSKYDashboardDataCollector.SysAid
{
    public class ServiceRequest
    {
        public int id { get; set; }
        public string computerID { get; set; }
        public string category_1 { get; set; }
        public string category_2 { get; set; }
        public string category_3 { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string assignedTo { get; set; }
        public string resolution { get; set; }
        public string solution { get; set; }
        public string lastUpdatedBy { get; set; }
        public string requestedBy { get; set; }
        public int statusID { get; set; }
        public int locationID { get; set; }

        public string location { get; set; }
        public string priority { get; set; }
        public string urgency { get; set; }
        public string status { get; set; }

        public string timeInsertedString { get; set; }
        public string timeUpdatedString { get; set; }
        public string timeClosedString { get; set; }

        public bool isClosed { get; set; }

        /// <summary>
        /// Returns the time between when the ticket was entered and when it was modified or closed
        /// </summary>
        public TimeSpan ResponseTime
        {
            get
            {
                return this.timeUpdated.Subtract(this.timeInserted);
                

            }
        }

        public DateTime timeInserted
        {
            get
            {
                DateTime returnMe = DateTime.MinValue;

                if (!string.IsNullOrEmpty(timeInsertedString))
                {
                    if (!DateTime.TryParse(timeInsertedString, out returnMe))
                    {
                        return DateTime.MinValue;
                    }
                }

                return returnMe;

            }

            set
            {
                timeInsertedString = value.ToString();
            }
        }

        public DateTime timeUpdated
        {
            get
            {
                DateTime returnMe = DateTime.MinValue;

                if (!string.IsNullOrEmpty(timeUpdatedString))
                {
                    if (!DateTime.TryParse(timeUpdatedString, out returnMe))
                    {
                        return DateTime.MinValue;
                    }
                }

                return returnMe;

            }

            set
            {
                timeUpdatedString = value.ToString();
            }
        }

        public DateTime timeClosed
        {
            get
            {
                DateTime returnMe = DateTime.MinValue;

                if (!string.IsNullOrEmpty(timeClosedString))
                {
                    if (!DateTime.TryParse(timeClosedString, out returnMe))
                    {
                        return DateTime.MinValue;
                    }
                }

                return returnMe;

            }
        }

        public ServiceRequest(int id, string location, int locationID, string urgency, string priority, string status, string computerID, string cat_1, string cat_2, string cat_3,
            string title, string description, string assignedTo, string resolution, string solution, string lastUpdatedBy,
            string requestedBy, string timeInserted, string timeUpdated, string timeClosed, bool closed)
        {
            this.id = id;
            this.status = status;
            this.computerID = computerID;
            this.category_1 = cat_1;
            this.category_2 = cat_2;
            this.category_3 = cat_3;
            this.title = title;
            this.description = description;
            this.assignedTo = assignedTo;
            this.resolution = resolution;
            this.solution = solution;
            this.lastUpdatedBy = lastUpdatedBy;
            this.requestedBy = requestedBy;
            this.timeInsertedString = timeInserted;
            this.timeUpdatedString = timeUpdated;
            this.timeClosedString = timeClosed;
            this.location = location;
            this.locationID = locationID;
            this.urgency = urgency;
            this.priority = priority;
            this.status = status;
            this.isClosed = closed;

            if (string.IsNullOrEmpty(location))
            {
                this.location = "No location set";
            }

        }

        public static List<ServiceRequest> loadAllServiceRequests(SqlConnection connection)
        {
            List<ServiceRequest> returnMe = new List<ServiceRequest>();

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text, 
                CommandText = "SELECT * FROM LSKY_service_req;"
            };
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    int locationID = -1;
                    int.TryParse(dataReader["location_id"].ToString().Trim(), out locationID);

                    bool closed = CommonFunctions.ParseBool(dataReader["status_detail"].ToString().Trim());

                    returnMe.Add(new ServiceRequest(
                            int.Parse(dataReader["id"].ToString().Trim()),
                            dataReader["location"].ToString().Trim(),
                            locationID,
                            dataReader["urgency"].ToString().Trim(),
                            dataReader["priority"].ToString().Trim(),
                            dataReader["status"].ToString().Trim(),
                            dataReader["computer_id"].ToString().Trim(),
                            dataReader["problem_type"].ToString().Trim(),
                            dataReader["problem_sub_type"].ToString().Trim(),
                            dataReader["third_level_category"].ToString().Trim(),
                            dataReader["title"].ToString().Trim(),
                            dataReader["description"].ToString().Trim(),
                            dataReader["responsibility"].ToString().Trim(),
                            dataReader["resolution"].ToString().Trim(),
                            dataReader["solution"].ToString().Trim(),
                            dataReader["update_user"].ToString().Trim(),
                            dataReader["request_user"].ToString().Trim(),
                            dataReader["insert_time"].ToString().Trim(),
                            dataReader["update_time"].ToString().Trim(),
                            dataReader["close_time"].ToString().Trim(),
                            closed
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<ServiceRequest> loadAllServiceRequests(SqlConnection connection, DateTime start, DateTime end)
        {
            List<ServiceRequest> returnMe = new List<ServiceRequest>();

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text, 
                CommandText = "SELECT * FROM LSKY_service_req WHERE insert_time>=@TimeFrom AND insert_time<=@TimeTo;"
            };
            sqlCommand.Parameters.AddWithValue("@TimeFrom", start);
            sqlCommand.Parameters.AddWithValue("@TimeTo", end);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    int locationID = -1;
                    int.TryParse(dataReader["location_id"].ToString().Trim(), out locationID);
                    
                    bool closed = CommonFunctions.ParseBool(dataReader["status_detail"].ToString().Trim());

                    returnMe.Add(new ServiceRequest(
                            int.Parse(dataReader["id"].ToString().Trim()),
                            dataReader["location"].ToString().Trim(),
                            locationID,
                            dataReader["urgency"].ToString().Trim(),
                            dataReader["priority"].ToString().Trim(),
                            dataReader["status"].ToString().Trim(),
                            dataReader["computer_id"].ToString().Trim(),
                            dataReader["problem_type"].ToString().Trim(),
                            dataReader["problem_sub_type"].ToString().Trim(),
                            dataReader["third_level_category"].ToString().Trim(),
                            dataReader["title"].ToString().Trim(),
                            dataReader["description"].ToString().Trim(),
                            dataReader["responsibility"].ToString().Trim(),
                            dataReader["resolution"].ToString().Trim(),
                            dataReader["solution"].ToString().Trim(),
                            dataReader["update_user"].ToString().Trim(),
                            dataReader["request_user"].ToString().Trim(),
                            dataReader["insert_time"].ToString().Trim(),
                            dataReader["update_time"].ToString().Trim(),
                            dataReader["close_time"].ToString().Trim(),
                            closed
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        public static List<ServiceRequest> loadOpenServiceRequests(SqlConnection connection)
        {
            List<ServiceRequest> returnMe = new List<ServiceRequest>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_service_req WHERE status_detail=0;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    int locationID = -1;
                    int.TryParse(dataReader["location_id"].ToString().Trim(), out locationID);


                    bool closed = CommonFunctions.ParseBool(dataReader["status_detail"].ToString().Trim());

                    returnMe.Add(new ServiceRequest(
                            int.Parse(dataReader["id"].ToString().Trim()),
                            dataReader["location"].ToString().Trim(),
                            locationID,
                            dataReader["urgency"].ToString().Trim(),
                            dataReader["priority"].ToString().Trim(),
                            dataReader["status"].ToString().Trim(),
                            dataReader["computer_id"].ToString().Trim(),
                            dataReader["problem_type"].ToString().Trim(),
                            dataReader["problem_sub_type"].ToString().Trim(),
                            dataReader["third_level_category"].ToString().Trim(),
                            dataReader["title"].ToString().Trim(),
                            dataReader["description"].ToString().Trim(),
                            dataReader["responsibility"].ToString().Trim(),
                            dataReader["resolution"].ToString().Trim(),
                            dataReader["solution"].ToString().Trim(),
                            dataReader["update_user"].ToString().Trim(),
                            dataReader["request_user"].ToString().Trim(),
                            dataReader["insert_time"].ToString().Trim(),
                            dataReader["update_time"].ToString().Trim(),
                            dataReader["close_time"].ToString().Trim(),
                            closed
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        public static List<ServiceRequest> loadOpenServiceRequests(SqlConnection connection, DateTime start, DateTime end)
        {
            List<ServiceRequest> returnMe = new List<ServiceRequest>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_service_req WHERE status_detail=0 AND insert_time>=@TimeFrom AND insert_time<=@TimeTo;";
            sqlCommand.Parameters.AddWithValue("@TimeFrom", start);
            sqlCommand.Parameters.AddWithValue("@TimeTo", end);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    int locationID = -1;
                    int.TryParse(dataReader["location_id"].ToString().Trim(), out locationID);

                    
                    bool closed = CommonFunctions.ParseBool(dataReader["status_detail"].ToString().Trim());

                    returnMe.Add(new ServiceRequest(
                            int.Parse(dataReader["id"].ToString().Trim()),
                            dataReader["location"].ToString().Trim(),
                            locationID,
                            dataReader["urgency"].ToString().Trim(),
                            dataReader["priority"].ToString().Trim(),
                            dataReader["status"].ToString().Trim(),
                            dataReader["computer_id"].ToString().Trim(),
                            dataReader["problem_type"].ToString().Trim(),
                            dataReader["problem_sub_type"].ToString().Trim(),
                            dataReader["third_level_category"].ToString().Trim(),
                            dataReader["title"].ToString().Trim(),
                            dataReader["description"].ToString().Trim(),
                            dataReader["responsibility"].ToString().Trim(),
                            dataReader["resolution"].ToString().Trim(),
                            dataReader["solution"].ToString().Trim(),
                            dataReader["update_user"].ToString().Trim(),
                            dataReader["request_user"].ToString().Trim(),
                            dataReader["insert_time"].ToString().Trim(),
                            dataReader["update_time"].ToString().Trim(),
                            dataReader["close_time"].ToString().Trim(),
                            closed
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }
        
        public static List<ServiceRequest> loadTicketsClosed(SqlConnection connection, DateTime start, DateTime end)
        {
            List<ServiceRequest> returnMe = new List<ServiceRequest>();

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text, 
                CommandText = "SELECT * FROM LSKY_service_req WHERE status_detail=1 AND close_time>=@TimeFrom AND close_time<=@TimeTo;"
            };
            sqlCommand.Parameters.AddWithValue("@TimeFrom", start);
            sqlCommand.Parameters.AddWithValue("@TimeTo", end);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    int locationID = -1;
                    int.TryParse(dataReader["location_id"].ToString().Trim(), out locationID);
                    
                    bool closed = CommonFunctions.ParseBool(dataReader["status_detail"].ToString().Trim());

                    returnMe.Add(new ServiceRequest(
                            int.Parse(dataReader["id"].ToString().Trim()),
                            dataReader["location"].ToString().Trim(),
                            locationID,
                            dataReader["urgency"].ToString().Trim(),
                            dataReader["priority"].ToString().Trim(),
                            dataReader["status"].ToString().Trim(),
                            dataReader["computer_id"].ToString().Trim(),
                            dataReader["problem_type"].ToString().Trim(),
                            dataReader["problem_sub_type"].ToString().Trim(),
                            dataReader["third_level_category"].ToString().Trim(),
                            dataReader["title"].ToString().Trim(),
                            dataReader["description"].ToString().Trim(),
                            dataReader["responsibility"].ToString().Trim(),
                            dataReader["resolution"].ToString().Trim(),
                            dataReader["solution"].ToString().Trim(),
                            dataReader["update_user"].ToString().Trim(),
                            dataReader["request_user"].ToString().Trim(),
                            dataReader["insert_time"].ToString().Trim(),
                            dataReader["update_time"].ToString().Trim(),
                            dataReader["close_time"].ToString().Trim(),
                            closed
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }
        
        public static List<ServiceRequest> loadNewestOpenServiceRequests(SqlConnection connection, int count)
        {
            List<ServiceRequest> returnMe = new List<ServiceRequest>();

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text,
                CommandText = "SELECT TOP " + count + " * FROM LSKY_service_req WHERE status_detail=0 ORDER BY insert_time DESC;"
            };
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    int locationID = -1;
                    int.TryParse(dataReader["location_id"].ToString().Trim(), out locationID);
                    
                    bool closed = CommonFunctions.ParseBool(dataReader["status_detail"].ToString().Trim());

                    returnMe.Add(new ServiceRequest(
                            int.Parse(dataReader["id"].ToString().Trim()),
                            dataReader["location"].ToString().Trim(),
                            locationID,
                            dataReader["urgency"].ToString().Trim(),
                            dataReader["priority"].ToString().Trim(),
                            dataReader["status"].ToString().Trim(),
                            dataReader["computer_id"].ToString().Trim(),
                            dataReader["problem_type"].ToString().Trim(),
                            dataReader["problem_sub_type"].ToString().Trim(),
                            dataReader["third_level_category"].ToString().Trim(),
                            dataReader["title"].ToString().Trim(),
                            dataReader["description"].ToString().Trim(),
                            dataReader["responsibility"].ToString().Trim(),
                            dataReader["resolution"].ToString().Trim(),
                            dataReader["solution"].ToString().Trim(),
                            dataReader["update_user"].ToString().Trim(),
                            dataReader["request_user"].ToString().Trim(),
                            dataReader["insert_time"].ToString().Trim(),
                            dataReader["update_time"].ToString().Trim(),
                            dataReader["close_time"].ToString().Trim(),
                            closed
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        public static int loadOpenRequestCount(SqlConnection connection)
        {
            int returnMe = 0;

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text, 
                CommandText = "SELECT COUNT(id) AS open_count FROM LSKY_service_req WHERE status_detail=0;"
            };
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = int.Parse(dataReader["open_count"].ToString().Trim());
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        public static int loadTicketCountByStatus(SqlConnection connection, string status)
        {
            int returnMe = 0;

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text, 
                CommandText = "SELECT COUNT(id) AS open_count FROM LSKY_service_req WHERE status='" + status + "';"
            };
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = int.Parse(dataReader["open_count"].ToString().Trim());
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        public static int loadClosedRequestCount(SqlConnection connection)
        {
            int returnMe = 0;

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text, 
                CommandText = "SELECT COUNT(id) AS closed_count FROM LSKY_service_req WHERE status_detail!=0;"
            };
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = int.Parse(dataReader["closed_count"].ToString().Trim());
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        public static int loadNumberOfNewTickets(SqlConnection connection, DateTime from, DateTime to)
        {
            int returnMe = 0;

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text, 
                CommandText = "SELECT COUNT(id) AS ticket_count FROM LSKY_service_req WHERE insert_time>=@TimeFrom AND insert_time<=@TimeTo;"
            };
            sqlCommand.Parameters.AddWithValue("@TimeFrom", from);
            sqlCommand.Parameters.AddWithValue("@TimeTo", to);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = int.Parse(dataReader["ticket_count"].ToString().Trim());
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        /* Loads the tickets whos closed date is between the specified dates */
        public static int loadNumberOfTicketCloses(SqlConnection connection, DateTime from, DateTime to)
        {
            int returnMe = 0;

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text, 
                CommandText = "SELECT COUNT(id) AS ticket_count FROM LSKY_service_req WHERE status_detail!=0 AND close_time>=@TimeFrom AND close_time<=@TimeTo;"
            };
            sqlCommand.Parameters.AddWithValue("@TimeFrom", from);
            sqlCommand.Parameters.AddWithValue("@TimeTo", to);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = int.Parse(dataReader["ticket_count"].ToString().Trim());
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }
    }
}