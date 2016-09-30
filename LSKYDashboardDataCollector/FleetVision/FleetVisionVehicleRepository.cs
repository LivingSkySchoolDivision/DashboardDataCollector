using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.FleetVision
{
    public class FleetVisionVehicleRepository
    {
        private readonly Dictionary<int, FleetVisionVehicle> _cache;

        private readonly string SQLStart = "SELECT " +
                                                 "vehsite " +
                                                 ",vehicle " +
                                                 ",RecordID " +
                                                 ",account " +
                                                 ",vehclass " +
                                                 ",WheelbaseID " +
                                                 ",driver " +
                                                 ",descript " +
                                                 ",VIN " +
                                                 ",plate1 " +
                                                 ",plate2 " +
                                                 ",fuelcard " +
                                                 ",priority " +
                                                 ",specneeds " +
                                                 ",inactive " +
                                                 ",modelyear " +
                                                 ",template " +
                                                 ",serialno " +
                                                 ",InFleet " +
                                                 ",ArchivedFPData " +
                                                 ",PMLevel " +
                                                 ",AutoFuelMeter " +
                                                 ",FuelAccount " +
                                                 ",RPLastUpdate " +
                                                 ",TTLastUpdate " +
                                                 ",OSLastUpdate " +
                                                 ",Lists1.Item AS VehicleClass " +
                                                 ",Lists2.Item AS FuelType " +
                                                 ",Lists3.Item AS Make " +
                                                 ",Lists4.Item AS Model " +
                                            "FROM Vehicles  " +
                                                 "LEFT OUTER JOIN Lists as Lists1 ON Vehicles.vehclass = Lists1.ItemId " +
                                                 "LEFT OUTER JOIN Lists as Lists2 ON Vehicles.fueltype = Lists2.ItemId " +
                                                 "LEFT OUTER JOIN Lists as Lists3 ON Vehicles.make = Lists3.ItemId " +
                                                 "LEFT OUTER JOIN Lists as Lists4 ON Vehicles.model = Lists4.ItemId ";

        public FleetVisionVehicleRepository()
        {
            _cache = new Dictionary<int, FleetVisionVehicle>();

            using (SqlConnection connection = new SqlConnection(Settings.DBConnectionString_FleetVision))
            {
                SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = SQLStart
                };
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        FleetVisionVehicle vehicle = dataReaderToVehicle(dbDataReader);
                        if (vehicle != null)
                        {
                            _cache.Add(vehicle.RecordID, vehicle);
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }

        }

        public FleetVisionVehicle dataReaderToVehicle(SqlDataReader dataReader)
        {
            return new FleetVisionVehicle()
            {
                RecordID = Parsers.ParseInt(dataReader["RecordID"].ToString().Trim()),
                VehicleNumber = dataReader["vehicle"].ToString().Trim(),
                Class= dataReader["VehicleClass"].ToString().Trim(),
                VIN= dataReader["VIN"].ToString().Trim(),
                Description= dataReader["descript"].ToString().Trim(),
                Plate1 = dataReader["plate1"].ToString().Trim(),
                Plate2 = dataReader["plate2"].ToString().Trim(),
                Driver = dataReader["driver"].ToString().Trim(),
                FuelType = dataReader["FuelType"].ToString().Trim(),
                IsActive = !Parsers.ParseBool(dataReader["inactive"].ToString().Trim()),
                Make= dataReader["Make"].ToString().Trim(),
                Model = dataReader["Model"].ToString().Trim(),
                ModelYear = dataReader["modelyear"].ToString().Trim(),
            };
        }

        public FleetVisionVehicle Get(int vehicleID)
        {
            if (_cache.ContainsKey(vehicleID))
            {
                return _cache[vehicleID];
            }
            else
            {
                return null;
            }
        }

    }
}