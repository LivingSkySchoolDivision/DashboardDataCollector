using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.SysAid
{
    public class SysAidUser
    {
        public string givenName { get; set; }
        public string sn { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public bool isEnabled { get; set; }
        public bool isAdministrator { get; set; }

        public SysAidUser(string givenName, string sn, string username, string email, bool isEnabled, bool isAdministrator)
        {
            this.givenName = givenName;
            this.sn = sn;
            this.username = username;
            this.email = email;
            this.isEnabled = isEnabled;
            this.isAdministrator = isAdministrator;
        }

        public static List<SysAidUser> loadAllUsers(SqlConnection connection)
        {
            List<SysAidUser> returnMe = new List<SysAidUser>();

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text, 
                CommandText = "SELECT * FROM LSKY_Users;"
            };
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    bool isEnabled = false;

                    if (dataReader["disable"].ToString().Trim().ToLower() == "y")
                    {
                        isEnabled = false;
                    }
                    else
                    {
                        isEnabled = true;
                    }

                    bool isAdministrator = false;
                    if (dataReader["administrator"].ToString().Trim().ToLower() == "y")
                    {
                        isAdministrator = true;
                    }
                    else
                    {
                        isAdministrator = false;
                    }

                    returnMe.Add(new SysAidUser(
                            dataReader["first_name"].ToString().Trim(),
                            dataReader["last_name"].ToString().Trim(),
                            dataReader["login_user"].ToString().Trim(),
                            dataReader["email_address"].ToString().Trim(),
                            isEnabled,
                            isAdministrator
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        public static int loadEndUserCount(SqlConnection connection)
        {
            int returnMe = 0;

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text, 
                CommandText = "SELECT COUNT(user_name) AS user_count FROM LSKY_USERS WHERE administrator='N' AND disable='N';"
            };
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = int.Parse(dataReader["user_count"].ToString().Trim());
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        public static int loadAdminCount(SqlConnection connection)
        {
            return loadTechnicianCount(connection);
        }

        public static int loadTechnicianCount(SqlConnection connection)
        {
            int returnMe = 0;

            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = connection, 
                CommandType = CommandType.Text, 
                CommandText = "SELECT COUNT(user_name) AS user_count FROM LSKY_USERS WHERE administrator='Y' AND disable='N';"
            };
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = int.Parse(dataReader["user_count"].ToString().Trim());
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

    }
}