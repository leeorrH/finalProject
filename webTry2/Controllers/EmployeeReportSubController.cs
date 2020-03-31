using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webTry2.Models;
using webTry2.Models.requests;
using System.IO;

namespace webTry2.Controllers
{
    public class EmployeeReportSubController : DBController
    {
        public EmployeeReportSubController()
        {
            connectToSQL();
        }
        

        public void SendMonthlyReport(EmpReport empReport)
        {

            sqlQuery = "INSERT INTO dbo.EmployeeReport (reportType,reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus)" +
                          " VALUES('" + empReport.reportType + "', '" + empReport.reportOwner + "', '" + DateToString(empReport) + "', '" + empReport.enc.serialNumber + "', NULL, NULL, NULL,'" + empReport.notifications + "', NULL, 'false'); ";
            var res=sqlIUDoperation(sqlQuery); 
            if (res == 0)
            {
                throw new System.InvalidOperationException("operation FAILED! NO ROWS HAS BEEN EFFECTED");
            }
            return ;
        }

        public void ChangingEncLocationReport(EmpReport empReport)
        {
            //getting location ID  

            empReport.enc.deviceLocation = SetLocationID(empReport.enc.deviceLocation);
            
            if (empReport.enc.deviceLocation != null)
            {
                sqlQuery = "INSERT INTO dbo.EmployeeReport (reportType,reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus) " +
                        "OUTPUT inserted.reportID " +
                        "VALUES('" + empReport.reportType + "', '" + empReport.reportOwner + "', '" + DateToString(empReport) + "', '" + empReport.enc.serialNumber + "', NULL, NULL,'" +empReport.enc.deviceLocation.locationID+ "' ,'" + empReport.notifications + "', NULL, 'false'); ";

                var res = sqlIUDoperation(sqlQuery); // TODO if false then ... 
                if (res == 0)
                {
                    throw new System.InvalidOperationException("operation FAILED! NO ROWS HAS BEEN EFFECTED");
                }
            }
            return;
        }

        public void DeliverToEmpRepord (EmpReport empReport)
        {
             empReport.enc.deviceLocation = SetLocationID(empReport.enc.deviceLocation);
            sqlQuery = "INSERT INTO dbo.EmployeeReport (reportType,reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus) " +
                        "OUTPUT inserted.reportID " +
                        "VALUES('" + empReport.reportType + "', '" + empReport.reportOwner + "', '" + DateToString(empReport) + "', '" + empReport.enc.serialNumber + "', '" + empReport.enc.ownerID + "', NULL,'" + empReport.enc.deviceLocation.locationID + "' ,'" + empReport.notifications + "', NULL, 'false'); ";

            var res = sqlIUDoperation(sqlQuery); // TODO if false then ... 
            if (res == 0)
            {
                throw new System.InvalidOperationException("operation FAILED! NO ROWS HAS BEEN EFFECTED");
            }
            return;
        }

        public List<EmpReport> GetEmpReports (string userName)
        {
            List<EmpReport> reports = new List<EmpReport>();
           
            sqlQuery = "SELECT * " +
                       "FROM EmployeeReport AS ER " +
                       "WHERE ER.reportOwner = '" + userName + "' " +
                       "AND MONTH(ER.date) = MONTH(GETDATE()) " +
                       "AND YEAR(ER.date) = YEAR(GETDATE());"; // this query return only reports that relevant to current month

            dataReader = sendSqlQuery(sqlQuery);

            reports = GetEmpReportsList(dataReader);

            closeConnectionAndReading();

            if (reports.Count > 0) return (reports);
            else
            {
                return null;
            }
        }
        
        public List<EmpReport> GetAllReports()
        {
            List<EmpReport> reports = null;

            sqlQuery = "SELECT * FROM EmployeeReport AS ER "+
                       "WHERE MONTH(ER.date) = MONTH(GETDATE()) " +
                       "AND YEAR(ER.date) = YEAR(GETDATE());";
            dataReader = sendSqlQuery(sqlQuery);

            reports = GetEmpReportsList(dataReader);

            closeConnectionAndReading();

            if (reports.Count > 0) return (reports);
            else
            {
                return null;
            }
        }
        //orit
        public void changingStatus(EmpReport empReport)
        {
            sqlQuery = "INSERT INTO dbo.EmployeeReport (reportType,reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus)" +
                     " VALUES('" + empReport.reportType + "', '" + empReport.reportOwner + "', '" + DateToString(empReport) + "', '" + empReport.enc.serialNumber + "', NULL, NULL, NULL,'" + empReport.notifications + "','"+empReport.reference+"', 'false'); ";
            var res = sqlIUDoperation(sqlQuery);
            if (res == 0)
            {
                throw new System.InvalidOperationException("operation FAILED! NO ROWS HAS BEEN EFFECTED");
            }
            return;





            /*
            SqlCommand command;
            SqlConnection connection = new SqlConnection(connectionString);
            command = new SqlCommand("INSERT INTO FileTable (File) Values(@File)", connection);
            command.Parameters.Add("@File", SqlDbType.Binary, file.Length).Value = file;
            connection.Open();
            command.ExecuteNonQuery();
            */

        }

        private string DateToString(EmpReport rep)
        {
            return rep.date.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        private Location SetLocationID(Location deviceLocation)
        {
            sqlQuery = "SELECT locationID " +
                        "FROM Locations L " +
                        "WHERE L.facility = '" + deviceLocation.facility
                        + "'  AND L.building = '" + deviceLocation.building
                        + "'  AND L.floor = " + deviceLocation.floor
                        + "   AND L.room = " + deviceLocation.room + ";" ;

            dataReader = sendSqlQuery(sqlQuery);

            if (dataReader.Read())
            {
                deviceLocation.locationID = Int32.Parse(dataReader.GetValue(0).ToString());
                closeConnectionAndReading();
            }
            else return null;

            return deviceLocation;
        }

        private List<EmpReport> GetEmpReportsList (SqlDataReader dataReader)
        {
            List<EmpReport> reports = new List<EmpReport>();
            EmpReport temp;

            while (dataReader.Read())
            {
                temp = new EmpReport();
                temp.reportID = Int32.Parse(dataReader.GetValue(0).ToString()); // V
                temp.reportType = dataReader.GetValue(1).ToString(); // V
                temp.reportOwner = dataReader.GetValue(2).ToString();  // V
                temp.date = DateTime.Parse(dataReader.GetValue(3).ToString()); //V
                temp.enc.serialNumber = dataReader.GetValue(4).ToString(); // V

                switch (temp.reportType)
                {
                    case "changing encryptor location":
                        temp.enc.deviceLocation.locationID = Int16.Parse(dataReader.GetValue(7).ToString());
                        break;

                    case "deliver to employee":
                        temp.enc.ownerID = dataReader.GetValue(5).ToString();
                        temp.enc.deviceLocation.locationID = Int16.Parse(dataReader.GetValue(7).ToString());
                        break;
                    case "changing encryptor status":
                        temp.enc.status = dataReader.GetValue(6).ToString();
                        temp.reference = null; // until sending file will be done
                        break;
                    default:
                        break;
                }

                if (!dataReader.GetValue(8).Equals(null))
                {
                    temp.notifications = dataReader.GetValue(8).ToString(); // can be null 
                }
               
                temp.approvementStatus = bool.Parse(dataReader.GetValue(10).ToString());
                reports.Add(temp);
            }
            
            return reports;
        }




    }
}