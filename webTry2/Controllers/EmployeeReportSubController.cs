using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webTry2.Models;
using webTry2.Models.requests;

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
                          " VALUES('" + empReport.reportType + "', '" + empReport.reportOwner + "', '" + dateToString(empReport) + "', '" + empReport.enc.serialNumber + "', NULL, NULL, NULL,'" + empReport.notifications + "', NULL, 'false'); ";
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

            empReport.enc.deviceLocation = setLocationID(empReport.enc.deviceLocation);
            
            if (empReport.enc.deviceLocation != null)
            {
                sqlQuery = "INSERT INTO dbo.EmployeeReport (reportType,reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus) " +
                        "OUTPUT inserted.reportID " +
                        "VALUES('" + empReport.reportType + "', '" + empReport.reportOwner + "', '" + dateToString(empReport) + "', '" + empReport.enc.serialNumber + "', NULL, NULL,'" +empReport.enc.deviceLocation.locationID+ "' ,'" + empReport.notifications + "', NULL, 'false'); ";

                var res = sqlIUDoperation(sqlQuery); // TODO if false then ... 
                if (res == 0)
                {
                    throw new System.InvalidOperationException("operation FAILED! NO ROWS HAS BEEN EFFECTED");
                }
            }
            return;
        }

        public void deliverToEmpRepord (EmpReport empReport)
        {
             empReport.enc.deviceLocation = setLocationID(empReport.enc.deviceLocation);
            sqlQuery = "INSERT INTO dbo.EmployeeReport (reportType,reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus) " +
                        "OUTPUT inserted.reportID " +
                        "VALUES('" + empReport.reportType + "', '" + empReport.reportOwner + "', '" + dateToString(empReport) + "', '" + empReport.enc.serialNumber + "', '" + empReport.enc.ownerID + "', NULL,'" + empReport.enc.deviceLocation.locationID + "' ,'" + empReport.notifications + "', NULL, 'false'); ";

            var res = sqlIUDoperation(sqlQuery); // TODO if false then ... 
            if (res == 0)
            {
                throw new System.InvalidOperationException("operation FAILED! NO ROWS HAS BEEN EFFECTED");
            }
            return;
        }

        private string dateToString(EmpReport rep)
        {
            return rep.date.ToString("yyyy-MM-dd HH:mm:ss.fff");
        } 

        private Location setLocationID(Location deviceLocation)
        {
            sqlQuery = "SELECT locationID " +
                        "FROM Locations L " +
                        "WHERE L.facility = '" + deviceLocation.facility
                        + "'  AND L.building = '" + deviceLocation.building
                        + "'  AND L.floor = " + deviceLocation.floor
                        + "   AND L.room = " + deviceLocation.room + "; ";
            SqlDataReader result = sendSqlQuery(sqlQuery);
            if (result.Read())
            {
                deviceLocation.locationID = Int32.Parse(result.GetValue(0).ToString());
                result.Close();
            }
            else return null;

            return deviceLocation;
        }










    }
}