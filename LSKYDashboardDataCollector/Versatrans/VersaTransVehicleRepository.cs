using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.Versatrans
{
    public class VersaTransVehicleRepository
    {
        private readonly Dictionary<int, List<int>> _vehiclesWithDrivers;
        private readonly Dictionary<int, List<int>> _employeesWithVehicles;
        private readonly Dictionary<int, VersaTransVehicle> _vehicles;

        public VersaTransVehicleRepository()
        {
            _vehiclesWithDrivers = new Dictionary<int, List<int>>();
            _employeesWithVehicles = new Dictionary<int, List<int>>();
            _vehicles = new Dictionary<int, VersaTransVehicle>();

            using (SqlConnection connection = new SqlConnection(Settings.DBConnectionString_Versatrans))
            {
                // Drivers
                using (SqlCommand sqlCommand = new SqlCommand()
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT VehicleID, EmployeeID FROM VehicleDriver"
                })
                {
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            int vehicleID = Parsers.ParseInt(dbDataReader["VehicleID"].ToString().Trim());
                            int employeeID = Parsers.ParseInt(dbDataReader["EmployeeID"].ToString().Trim());

                            if (!_vehiclesWithDrivers.ContainsKey(vehicleID))
                            {
                                _vehiclesWithDrivers.Add(vehicleID, new List<int>());
                            }

                            if (!_vehiclesWithDrivers[vehicleID].Contains(employeeID))
                            {
                                _vehiclesWithDrivers[vehicleID].Add(employeeID);
                            }

                            if (!_employeesWithVehicles.ContainsKey(employeeID))
                            {
                                _employeesWithVehicles.Add(employeeID, new List<int>());
                            }

                            if (!_employeesWithVehicles[employeeID].Contains(vehicleID))
                            {
                                _employeesWithVehicles[employeeID].Add(vehicleID);
                            }

                        }
                    }
                    sqlCommand.Connection.Close();
                }


                // Vehicles
                using (SqlCommand sqlCommand = new SqlCommand()
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT RecordID, vehicle, VIN, plate1 FROM Vehicles"
                })
                {
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            VersaTransVehicle vehicle = dataReaderToVehicle(dbDataReader);
                            if (vehicle != null)
                            {
                                _vehicles.Add(vehicle.RecordID, vehicle);
                            }
                        }
                    }
                    sqlCommand.Connection.Close();
                }

                
            }
        }

        public List<VersaTransVehicle> GetForEmployee(int EmployeeID)
        {
            List<VersaTransVehicle> returnMe = new List<VersaTransVehicle>();

            if (_employeesWithVehicles.ContainsKey(EmployeeID))
            {
                foreach (int vehicleID in _employeesWithVehicles[EmployeeID])
                {
                    if (_vehicles.ContainsKey(vehicleID))
                    {
                        returnMe.Add(_vehicles[vehicleID]);
                    }
                }
            }

            return returnMe;
        }



        private VersaTransVehicle dataReaderToVehicle(SqlDataReader dataReader)
        {
            return new VersaTransVehicle()
            {
                RecordID = Parsers.ParseInt(dataReader["RecordID"].ToString().Trim()),
                VehicleNumber = dataReader["vehicle"].ToString().Trim(),
                VIN = dataReader["VIN"].ToString().Trim(),
                Plate1 = dataReader["plate1"].ToString().Trim()
            };
        }
    }
}