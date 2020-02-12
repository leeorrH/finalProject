using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace webTry2.Controllers
{
    public class EmployeeReportSubController : DBController
    {
        public EmployeeReportSubController()
        {
            connectToSQL();
        }
        

        public void SendMonthlyReport(List<string> empReport)
        {
            sqlQuery = "INSERT INTO dbo.EmployeeReport (reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus)" +
                          " VALUES('" + empReport[1] + "', '" + empReport[2] + "', '" + empReport[3] + "', NULL, NULL, NULL,'" + empReport[4] + "', NULL, 'false'); ";
            var res=sqlIUDoperation(sqlQuery); 
            if (res == 0)
            {
                throw new System.InvalidOperationException("operation FAILED! NO ROWS HAS BEEN EFFECTED");
            }
            return ;
        }

        public void ChangingEncLocationReport(List<string> empReport)
        {
            bool dataReaderFlag = false;
            //getting location ID
            sqlQuery = "SELECT locationID " +
                        "FROM Locations L " +
                        "WHERE L.facility = '" + empReport[5] + "' AND L.building = '" + empReport[6] + "'  AND L.floor = " + empReport[7] + " AND L.room = " + empReport[8] + "; ";

            SqlDataReader result = sendSqlQuery(sqlQuery);
            
            if (result.Read())
            {
                dataReaderFlag = true;
                var locationID = Int32.Parse(result.GetValue(0).ToString());
                result.Close();
                sqlQuery = "INSERT INTO dbo.EmployeeReport (reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus) " +
                        "OUTPUT inserted.reportID " +
                        "VALUES('" + empReport[1]/*reportOwner*/ + "', '" + empReport[2]/*date*/ + "', '" + empReport[3] /*encSN*/+ "', NULL" +/*ownerID*/", NULL," + locationID + ",'" + empReport[4]/*notifications*/ + "', NULL, 'false'); ";

                var res = sendSqlQuery(sqlQuery); // TODO if false then ... 
            }

            if (dataReaderFlag == false)
            {
                //TODO ..
            }

        }










    }
}