using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.SysAid
{
    public class SysAidAsset
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string ip { get; set; }
        public string description { get; set; }
        public string macAddress { get; set; }
        public string model { get; set; }
        public bool isDisabled { get; set; }
        public string agentVersion { get; set; }

        public SysAidAsset(string id, string name, string type, string model, string ip, string description, string mac, string agentversion, bool disabled)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.model = model;
            this.ip = ip;
            this.description = description;
            this.macAddress = mac;
            this.agentVersion = agentVersion;
            this.isDisabled = disabled;
        }

        public static int loadAssetCount(SqlConnection connection)
        {
            int returnMe = 0;

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT COUNT(computer_id) AS asset_count FROM LSKY_Assets WHERE disable='N';";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = int.Parse(dataReader["asset_count"].ToString().Trim());
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

        public static int loadOnlineAssetCount(SqlConnection connection)
        {
            int returnMe = 0;

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT COUNT(computer_id) AS asset_count FROM LSKY_OnlineAssets WHERE is_online=1;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = int.Parse(dataReader["asset_count"].ToString().Trim());
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

    }
}