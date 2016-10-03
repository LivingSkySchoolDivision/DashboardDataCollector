using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using LSKYDashboardDataCollector.FleetVision;

namespace LSKYDashboardDataCollector.Versatrans
{
    public class VersaTransEmployeeRepository
    {
        private readonly VersaTransVehicleRepository _vehicleRepo;
        private readonly VersaTransCertificationRepository _certificationRepo;

        private string SQLQuery = "SELECT RecordID, LastName, FirstName, EmployeeNo, Active FROM Employees ";


        private readonly Dictionary<int, VersaTransEmployee> _cache;
        public VersaTransEmployeeRepository()
        {
            _cache = new Dictionary<int, VersaTransEmployee>();

            _vehicleRepo = new VersaTransVehicleRepository();
            _certificationRepo = new VersaTransCertificationRepository();

            using (SqlConnection connection = new SqlConnection(Settings.DBConnectionString_Versatrans))
            {
                SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = SQLQuery
                };
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        VersaTransEmployee employee = dataReaderToEmployee(dbDataReader);
                        if (employee != null)
                        {
                            _cache.Add(employee.RecordID, employee);
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }
        }


        private VersaTransEmployee dataReaderToEmployee(SqlDataReader dataReader)
        {
            int RecordID = Parsers.ParseInt(dataReader["RecordID"].ToString().Trim());

            bool IsActive = dataReader["Active"].ToString().Trim().ToLower() == "y";

            return new VersaTransEmployee()
            {
                RecordID = RecordID,
                FirstName = dataReader["FirstName"].ToString().Trim(),
                LastName = dataReader["LastName"].ToString().Trim(),
                EmployeeNumber = dataReader["EmployeeNo"].ToString().Trim(),
                Certifications = _certificationRepo.GetForEmployee(RecordID),
                Vehicles = _vehicleRepo.GetForEmployee(RecordID),
                IsActive = IsActive
            };
        }

        public List<VersaTransEmployee> GetAllActive()
        {
            return _cache.Values.Where(x => x.IsActive == true).ToList();
        }


    }
}