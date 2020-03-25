using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using webTry2.Models;
using System.Globalization;

namespace webTry2.Controllers
{
    public class employeeReportSubController : DBController
    {
        public employeeReportSubController()
        {
            //connect to sql
            connectToSQL();
        }

        public List<Encryptor> loadEncryptorByUser(string userName)
        {
            List<Encryptor> userEncryptors = new List<Encryptor>();
             

            sqlQuery = "select enc.SN , CONVERT(varchar,enc.timeStamp,20) ,stat.statusName , loc.* " +
                         " from Encryptors as enc, Locations as loc , Status as stat " +
                          " where enc.ownerID = '" + userName + "' and enc.locationID = loc.locationID and enc.status = stat.statusID";

            command = new SqlCommand(sqlQuery, connectionToSql);

            dataReader = command.ExecuteReader();

            //adding data to classes by query
            while (dataReader.Read())
            {
                Encryptor temp = new Encryptor();

                //adding data to Encryptor
                temp.serialNumber = dataReader.GetValue(0).ToString();

                //setting date 
                temp.timestampAsString = dataReader.GetValue(1).ToString(); //date as string
                temp.timestamp = DateTime.ParseExact(temp.timestampAsString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); // date as an object


                temp.status = dataReader.GetValue(2).ToString();
                temp.ownerID = userName; //need to set as User class ! TBD

                //adding data to location
                temp.deviceLocation.locationID = Int16.Parse(dataReader.GetValue(3).ToString());
                temp.deviceLocation.facility = dataReader.GetValue(4).ToString();
                temp.deviceLocation.building = dataReader.GetValue(5).ToString();
                temp.deviceLocation.floor = UInt32.Parse(dataReader.GetValue(6).ToString());
                temp.deviceLocation.room = UInt32.Parse(dataReader.GetValue(7).ToString());
                temp.deviceLocation.longitude = Double.Parse(dataReader.GetValue(8).ToString());
                temp.deviceLocation.latitude = Double.Parse(dataReader.GetValue(9).ToString());

                //adding encryptor to user encryptors 
                userEncryptors.Add(temp);
            }


            closeConnectionAndReading();
            //return Json(userEncryptors, JsonRequestBehavior.AllowGet);
            return userEncryptors;
        }


    }
}