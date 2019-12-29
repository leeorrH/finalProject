using System;
using System.Web.Mvc;
using webTry2.Models;
using System.Data.SqlClient;

namespace webTry2.Controllers
{
    public class HomeController : DBController
    {
        User userReturn = new User();

        public HomeController()
        {
            
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginCheck(User userObj)
        {
            Boolean dataReaderFlag = false; 
            //User userReturn = new User();
            if(userObj.userName == null)
                return Json(userReturn, JsonRequestBehavior.AllowGet);

            connectToSQL();

            //sql auery for validate user name and password
            sqlQuery = "select *" +
                " from Users" +
                " where Users.userName = '"+userObj.userName + "'"
                + "and Users.password = '" + userObj.password + "'";

                 command = new SqlCommand(sqlQuery, connectionToSql);

                dataReader = command.ExecuteReader();
            
            //adding data to classes by query
            while (dataReader.Read())
            {
                dataReaderFlag = true;
                userReturn.userName = dataReader.GetValue(0).ToString();
                userReturn.password = dataReader.GetValue(1).ToString();
                userReturn.permission = dataReader.GetValue(6).ToString();
            }

            closeConnectionAndReading();

            //in case there is no such user in DB
            if (dataReaderFlag == false) return Json(null, JsonRequestBehavior.AllowGet);

            return Json(userReturn, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult userRegistration(User userObj)
        {
            connectToSQL();

            //need to pay attention to put ' ' between rows when writing sql query ! 
            sqlQuery = "INSERT INTO Users" +
                        " VALUES ('" + userObj.userName + "', " +
                                "'" + userObj.password + "', " +
                                "'" + userObj.firstName + "', " +
                                "'" + userObj.lastName + "', " +
                                "'" + userObj.email + "', " +
                                "'" + userObj.phoneNumber + "', " +
                                "'employee',"+
                                "'" + userObj.unit + "');";


            command = new SqlCommand(sqlQuery, connectionToSql);

            try
            {
                dataReader = command.ExecuteReader();
                closeConnectionAndReading();
                return Json("Registration Success", JsonRequestBehavior.AllowGet);
            }
            catch(SqlException e)
            {
                System.Console.Write(e.ToString());
                closeConnectionAndReading();
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }


    }
}