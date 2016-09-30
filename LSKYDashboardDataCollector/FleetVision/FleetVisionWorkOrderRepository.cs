using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using LSKYDashboardDataCollector.Common;
using LSKYDashboardDataCollector.Jira;

namespace LSKYDashboardDataCollector.FleetVision
{
    public class FleetVisionWorkOrderRepository
    {
        private static string SQLStart = "SELECT " +
                                             "Lists_1.Item as WOStatus, " +
                                             "vtUser_1.FirstName AS CreatedByFirstName, " +
                                             "vtUser_1.LastName AS CreatedByLastName, " +
                                             "vtUser_2.FirstName AS UpdatedByFirstName, " +
                                             "vtUser_2.LastName AS UpdatedByLastName, " +
                                             "WorkOrders.RecordID, " +
                                             "WorkOrders.WONumber, " +
                                             "WorkOrders.VehKey, " +
                                             "WorkOrders.InDateTime, " +
                                             "WorkOrders.Odometer, " +
                                             "WorkOrders.PartsTotal, " +
                                             "WorkOrders.LaborTotal, " +
                                             "WorkOrders.EstDateTime, " +
                                             "WorkOrders.CreatedTime, " +
                                             "WorkOrders.UpdatedTime, " +
                                             "WorkOrders.ShopFee, " +
                                             "WorkOrders.ShopFeeIsFlat, " +
                                             "WorkOrders.InvoiceNumber, " +
                                             "WorkOrders.WorkRequested, " +
                                             "WorkOrders.WorkPfmd, " +
                                             "WorkOrders.RequestBy, " +
                                             "Lists_2.Item as Priority " +
                                         "FROM WorkOrders " +
                                         "LEFT OUTER JOIN vtUser AS vtUser_2 ON WorkOrders.UpdatedBy = vtUser_2.ID " +
                                         "LEFT OUTER JOIN vtUser AS vtUser_1 ON WorkOrders.CreatedBy = vtUser_1.ID " +
                                         "LEFT OUTER JOIN Lists AS Lists_1 ON WorkOrders.Status = Lists_1.ItemId " +
                                         "LEFT OUTER JOIN Lists AS Lists_2 ON WorkOrders.Priority = Lists_2.ItemId ";

        private readonly Dictionary<int, FleetVisionWorkOrder> _cache;
        private readonly FleetVisionVehicleRepository _vehicleRepository;

        private FleetVisionWorkOrder dataReaderToWorkOrder(SqlDataReader dataReader)
        {
            string priority = dataReader["Priority"].ToString().Trim();

            if (string.IsNullOrEmpty(priority))
            {
                priority = string.Empty;
            }

            return new FleetVisionWorkOrder()
            {
                RecordID = Parsers.ParseInt(dataReader["RecordID"].ToString().Trim()),
                WorkOrderNumber = dataReader["WONumber"].ToString().Trim(),
                RequestBy = dataReader["RequestBy"].ToString().Trim(),
                VehicleRecordID = Parsers.ParseInt(dataReader["VehKey"].ToString().Trim()),
                WorkRequested = dataReader["WorkRequested"].ToString().Trim(),
                Status = dataReader["WOStatus"].ToString().Trim(),
                EstDateTime = Parsers.ParseDate(dataReader["EstDateTime"].ToString().Trim()),
                WorkPerformed = dataReader["WorkPfmd"].ToString().Trim(),
                PartsTotal = Parsers.ParseDecimal(dataReader["PartsTotal"].ToString().Trim()),
                LaborTotal = Parsers.ParseDecimal(dataReader["LaborTotal"].ToString().Trim()),
                CreatedBy = dataReader["CreatedTime"].ToString().Trim(),
                LastUpdated = Parsers.ParseDate(dataReader["UpdatedTime"].ToString().Trim()),
                ShopFee = Parsers.ParseDecimal(dataReader["ShopFee"].ToString().Trim()),
                InvoiceNumber = dataReader["InvoiceNumber"].ToString().Trim(),
                DateCreated = Parsers.ParseDate(dataReader["CreatedTime"].ToString().Trim()),
                _priority = priority,
                Vehicle = _vehicleRepository.Get(Parsers.ParseInt(dataReader["VehKey"].ToString().Trim())),
                InDateTime = Parsers.ParseDate(dataReader["InDateTime"].ToString().Trim())
            };
        }

        public FleetVisionWorkOrderRepository()
        {
            _vehicleRepository = new FleetVisionVehicleRepository();
            _cache = new Dictionary<int, FleetVisionWorkOrder>();

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
                        FleetVisionWorkOrder workOrder = dataReaderToWorkOrder(dbDataReader);
                        if (workOrder != null)
                        {
                            _cache.Add(workOrder.RecordID, workOrder);
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }
        }

        public List<FleetVisionWorkOrder> GetAll()
        {
            return _cache.Values.ToList();
        }

        public List<FleetVisionWorkOrder> GetAllOpen()
        {
            return _cache.Values.Where(wo => !wo.IsClosed).ToList();
        }

        public List<FleetVisionWorkOrder> GetAllClosed()
        {
            return _cache.Values.Where(wo => wo.IsClosed).ToList();
        }

        public List<FleetVisionWorkOrder> GetWorkOrdersCreatedDuring(DateTime timeFrom, DateTime timeTo)
        {
            return _cache.Values.Where(wo => wo.DateCreated >= timeFrom && wo.DateCreated <= timeTo).ToList();
        }

        public List<FleetVisionWorkOrder> GetAllRecent(int number)
        {
            List<FleetVisionWorkOrder> returnMe = new List<FleetVisionWorkOrder>();
            if (number > 0)
            {
                List<FleetVisionWorkOrder> workOrdersByDate =
                    _cache.Values.OrderByDescending(wo => wo.DateCreated).ToList();

                int count = number;
                if (workOrdersByDate.Count < number)
                {
                    count = workOrdersByDate.Count;
                }

                for (int x = 0; x < count; x++)
                {
                    returnMe.Add(workOrdersByDate[x]);
                }

            }
            return returnMe;
        }
        public List<FleetVisionWorkOrder> GetRecentIncomplete(int number)
        {
            List<FleetVisionWorkOrder> returnMe = new List<FleetVisionWorkOrder>();
            if (number > 0)
            {
                List<FleetVisionWorkOrder> workOrdersByDate =
                    _cache.Values.Where(wo => !wo.IsClosed).OrderByDescending(wo => wo.DateCreated).ToList();

                int count = number;
                if (workOrdersByDate.Count < number)
                {
                    count = workOrdersByDate.Count;
                }

                for (int x = 0; x < count; x++)
                {
                    returnMe.Add(workOrdersByDate[x]);
                }

            }
            return returnMe;
        }

    }
}