using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.Versatrans
{
    public class VersaTransCertificationRepository
    {
        private readonly Dictionary<int, List<VersatransCertification>> _certificationsByEmployee;

        private const string SQLQuery = "SELECT " +
                                            "EmployeeTraining.RecordID, " +
                                            "EmployeeTraining.EmployeeID, " +
                                            "EmployeeTraining.DateCompleted, " +
                                            "EmployeeTraining.ExpirationDate, " +
                                            "CertificationType.Description " +
                                        "FROM " +
                                            "EmployeeTraining " +
                                            "LEFT OUTER JOIN CertificationType ON EmployeeTraining.CertificationTypeID=CertificationType.RecordID ";

        public VersaTransCertificationRepository()
        {
            _certificationsByEmployee = new Dictionary<int, List<VersatransCertification>>();

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
                        VersatransCertification cert = dataReaderToCert(dbDataReader);
                        if (cert != null)
                        {
                            if (!_certificationsByEmployee.ContainsKey(cert.EmployeeID))
                            {
                                _certificationsByEmployee.Add(cert.EmployeeID, new List<VersatransCertification>());
                            }
                            _certificationsByEmployee[cert.EmployeeID].Add(cert);
                        }

                    }
                }

                sqlCommand.Connection.Close();
            }

        }
        
        private VersatransCertification dataReaderToCert(SqlDataReader dataReader)
        {
            return new VersatransCertification()
            {
                RecordID = Parsers.ParseInt(dataReader["RecordID"].ToString().Trim()),
                CertificationType = dataReader["Description"].ToString().Trim(),
                Completed = Parsers.ParseDate(dataReader["DateCompleted"].ToString().Trim()),
                Expires = Parsers.ParseDate(dataReader["ExpirationDate"].ToString().Trim()),
                EmployeeID = Parsers.ParseInt(dataReader["EmployeeID"].ToString().Trim())
            };
        }

        public List<VersatransCertification> GetForEmployee(int EmployeeID)
        {
            return _certificationsByEmployee.ContainsKey(EmployeeID) ? _certificationsByEmployee[EmployeeID] : new List<VersatransCertification>();
        }
    }
}