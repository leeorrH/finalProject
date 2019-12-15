using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using webTry2.Models;
using System.Globalization;
using System.Web.Mvc;

namespace webTry2.Controllers
{
    public class encryptorSubController : DBController
    {
        public encryptorSubController()
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
            //return Json(userEncryptors, JsonRequestBehavior.AllowGet);
            return userEncryptors;
        }

    }
}