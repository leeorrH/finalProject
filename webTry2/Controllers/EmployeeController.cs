using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using webTry2.Controllers;
using webTry2.Models;
using webTry2.Models.requests;

namespace WEB_project.Controllers
{
    public class EmployeeController : DBController
    {
        private EmployeeReportSubController empRepCtrl = new EmployeeReportSubController();

        public Dictionary<string,User> users = new Dictionary<string, User>();

        // GET: Employee
        public ActionResult employeePage()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult loadEmployeeEncryptors(string userName)
        {
            employeeReportSubController encCntrl = new employeeReportSubController();

            List<Encryptor> result = encCntrl.loadEncryptorByUser(userName);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost] //move to DB controller
        public ActionResult getEmployeeList(string empUserName)
        {
            List<User> employees = new List<User>();
            User temp;
            bool dataReaderFlag = false;

            connectToSQL();
            sqlQuery = "select emp.userName, emp.firstName, emp.lastName " +
                       " from Users as emp " +
                       " where emp.userName <> '" + empUserName + "'";

            SqlDataReader result = sendSqlQuery(sqlQuery);
            while (result.Read())
            {
                dataReaderFlag = true;
                temp = new User();
                temp.userName = result.GetValue(0).ToString();
                temp.firstName = result.GetValue(1).ToString();
                temp.lastName = result.GetValue(2).ToString();
                employees.Add(temp);
            }
            closeConnectionAndReading();

            if (dataReaderFlag == false)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//move to DB controller
        public ActionResult getBuildingList(string siteName)
        {
            List<string> buildings = new List<string>();
            string temp;
            bool dataReaderFlag = false;

            connectToSQL();
            sqlQuery = "select distinct L.building " +
                "from Locations L " +
                "where L.facility = '" + siteName + "'";

            SqlDataReader result = sendSqlQuery(sqlQuery);
            while (result.Read())
            {
                dataReaderFlag = true;
                temp = result.GetValue(0).ToString();
                buildings.Add(temp);
            }
            closeConnectionAndReading();

            if (dataReaderFlag == false)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(buildings, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]//move to DB controller
        public ActionResult getfloorsList(string siteName,string buildName)
        {
            List<string> floors = new List<string>();
            string temp;
            bool dataReaderFlag = false;

            connectToSQL();
            sqlQuery = "select distinct L.floor" +
                        " from Locations L" +
                        " where L.facility = '" + siteName + "'and L.building = '" + buildName + "';";

            SqlDataReader result = sendSqlQuery(sqlQuery);
            while (result.Read())
            {
                dataReaderFlag = true;
                temp = result.GetValue(0).ToString();
                floors.Add(temp);
            }
            closeConnectionAndReading();

            if (dataReaderFlag == false)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(floors, JsonRequestBehavior.AllowGet);
        }

        [HttpPost] //move to DB controller
        public ActionResult getRoomList(string siteName, string buildName, string floorNumber)
        {
            List<string> rooms = new List<string>();
            string temp;
            bool dataReaderFlag = false;

            connectToSQL();
            sqlQuery = "select distinct L.room"+
                        " from Locations L"+
                        " where L.floor = '"+floorNumber+"' and L.facility = '"+ siteName + "'and L.building = '"+ buildName + "'; ";

            SqlDataReader result = sendSqlQuery(sqlQuery);
            while (result.Read())
            {
                dataReaderFlag = true;
                temp = dataReader.GetValue(0).ToString();
                rooms.Add(temp);
            }
            closeConnectionAndReading();

            if (dataReaderFlag == false)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(rooms, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendReport(EmpReport empReport)
        {
            try
            {
                switch (empReport.reportType)
                {
                    case "monthly report":
                        empRepCtrl.SendMonthlyReport(empReport);
                        break;
                    case "changing encryptor location":
                        empRepCtrl.ChangingEncLocationReport(empReport);
                        break;
                    case "changing encryptor status":
                        empRepCtrl.changingStatus(empReport);
                        break;
                    case "deliver to employee":
                        empRepCtrl.DeliverToEmpRepord(empReport);
                        break;

                    default:
                        return Json("Error: request option not recognized", JsonRequestBehavior.AllowGet); ;
                }
                closeConnectionAndReading();

            }
            catch (Exception ex)
            {
                Console.WriteLine("sql fail");
                return Json("sql fail", JsonRequestBehavior.AllowGet);
            }
            return Json("sql success", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult getUserDetails(string userName)
        {
            bool dataReaderFlag = false;
            User user;
            if (users.TryGetValue(userName,out user))
            {
                dataReaderFlag = true;
            }
            else
            {
                user = new User();

                connectToSQL();

                sqlQuery = "select *" +
                    " from Users" +
                    " where Users.userName = '" + userName + "'";

                SqlDataReader result = sendSqlQuery(sqlQuery);
                while (result.Read())
                {
                    dataReaderFlag = true;
                    user.userName = dataReader.GetValue(0).ToString();
                    user.firstName = dataReader.GetValue(2).ToString();
                    user.lastName = dataReader.GetValue(3).ToString();
                    user.email = dataReader.GetValue(4).ToString();
                    user.phoneNumber = dataReader.GetValue(5).ToString();
                    user.permission = dataReader.GetValue(6).ToString();
                    user.unit = dataReader.GetValue(7).ToString();
                }

                users.Add(userName, user);
            }
            
            if (dataReaderFlag == false)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(user, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult getRequests(string userName , string permission)
        {
            List<EmpReport> res = null;
            if (permission.Equals("employee")) res = empRepCtrl.GetEmpReports(userName);
            else res = empRepCtrl.GetAllReports();
            if(res.Count > 0) return Json(res, JsonRequestBehavior.AllowGet);
            return Json(null , JsonRequestBehavior.AllowGet);
        }
    }

}


/* ---------------- things that have been moved to diffrent controllers
        private string connectionString;
        private SqlConnection connectionToSql;
        SqlCommand command;
        SqlDataReader dataReader;
        String sqlQuery; 

        public EmployeeController()
        {
            //initilaize parameters for controller
            connectionString = "Data Source=DESKTOP-7NV1617\\SQLEXPRESS;" +
                                      "Initial Catalog=project_DB;" +
                                         "Integrated Security=True";

            connectionToSql = new SqlConnection(connectionString);
        }

/-----------------------------------------------------------------------------------/

     [HttpPost]
        public ActionResult loadEmployeeEncryptors(String userName)
        {
                       
            List<Encryptor> userEncryptors = new List<Encryptor>();
            //connection to DB 
            try
            {
                connectionToSql.Open();

            }
            catch (SqlException)
            {
                Console.WriteLine("error while connecting to DB");
            }

            //query for getting encryptors by userNumber and thier location from the user
            sqlQuery = "select enc.SN , CONVERT(varchar,enc.timeStamp,20) ,stat.statusName , loc.* " +
                       " from Encryptors as enc, Locations as loc , Status as stat " +
                        " where enc.ownerID = '" + userName + "' and enc.locationID = loc.locationID and enc.status = stat.statusID";

            command = new SqlCommand(sqlQuery, connectionToSql);

            dataReader = command.ExecuteReader();

            //adding data to classes by query
            while (dataReader.Read())
            {
                Encryptor temp = new Encryptor();
                Location loc = new Location();

                //adding data to Encryptor
                temp.serialNumber = dataReader.GetValue(0).ToString();

                //setting date 
                temp.timestampAsString = dataReader.GetValue(1).ToString(); //date as string
                temp.timestamp = DateTime.ParseExact(temp.timestampAsString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); // date as an object
                

                temp.status = dataReader.GetValue(2).ToString();
                temp.ownerID = userName;

                //adding data to location
                loc.locationID = Int16.Parse(dataReader.GetValue(3).ToString());
                loc.facility = dataReader.GetValue(4).ToString();
                loc.building = dataReader.GetValue(5).ToString();
                loc.floor = UInt32.Parse(dataReader.GetValue(6).ToString());
                loc.room = UInt32.Parse(dataReader.GetValue(7).ToString());
                loc.longitude = Double.Parse(dataReader.GetValue(8).ToString());
                loc.latitude = Double.Parse(dataReader.GetValue(9).ToString());

                //seting location to encryptor
                temp.deviceLocation = loc;
                //adding encryptor to user encryptors 
                userEncryptors.Add(temp);
            }

            
            connectionToSql.Close();
            dataReader.Close();
            return Json(userEncryptors, JsonRequestBehavior.AllowGet);
        }

        */
