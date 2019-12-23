using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Mvc;
using webTry2.Models;

namespace webTry2.Controllers
{
    public class AdminController : Controller
    {
        private string connectionString;
        private SqlConnection connectionToSql;
        SqlCommand command;
        SqlDataReader dataReader;
        String sqlQuery;

        public AdminController()
        {
            //initilaize parameters for controller
            connectionString = "Data Source=DESKTOP-7NV1617\\SQLEXPRESS;" +
                                      "Initial Catalog=project_DB;" +
                                         "Integrated Security=True";

            connectionToSql = new SqlConnection(connectionString);
        }

        // GET: Admin
        public ActionResult adminPage()
        {
            return View();
        }

        //loadAllEncryptors - function that get all encryptors from SQL DB
        [HttpPost]
        public ActionResult loadAllEncryptors()
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

            //query for getting ALL encryptors and thier location from the user
            sqlQuery = "select enc.SN, enc.ownerID, CONVERT(varchar,enc.timeStamp,20) ,stat.statusName , loc.* " +
                       " from Encryptors as enc, Locations as loc , Status as stat " +
                        " where enc.locationID = loc.locationID and enc.status = stat.statusID";

            command = new SqlCommand(sqlQuery, connectionToSql);

            dataReader = command.ExecuteReader();

            //adding data to classes by query
            while (dataReader.Read())
            {
                Encryptor temp = new Encryptor();
                Location loc = new Location();

                /*              adding data to Encryptor                   */
               
                temp.serialNumber = dataReader.GetValue(0).ToString(); //Encryptor SN
                temp.ownerID = dataReader.GetValue(1).ToString(); // owner ID - employee user name
                //setting date 
                temp.timestampAsString = dataReader.GetValue(2).ToString(); //date as string
                temp.timestamp = DateTime.ParseExact(temp.timestampAsString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); // date as an object
                temp.status = dataReader.GetValue(3).ToString(); // 

                //adding data to location
                loc.locationID = Int16.Parse(dataReader.GetValue(4).ToString());// location id
                loc.facility = dataReader.GetValue(5).ToString();// facility
                loc.building = dataReader.GetValue(6).ToString();// building
                loc.floor = UInt32.Parse(dataReader.GetValue(7).ToString());// floor
                loc.room = UInt32.Parse(dataReader.GetValue(8).ToString());// room
                loc.longitude = Double.Parse(dataReader.GetValue(9).ToString());// lat
                loc.latitude = Double.Parse(dataReader.GetValue(10).ToString());// long
                //seting location to encryptor
                temp.deviceLocation = loc;
                //adding encryptor to user encryptors 
                userEncryptors.Add(temp);
            }


            connectionToSql.Close();
            dataReader.Close();
            return Json(userEncryptors, JsonRequestBehavior.AllowGet);
        }
    }

}