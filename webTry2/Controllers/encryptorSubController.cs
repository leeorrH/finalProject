using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using webTry2.Models;
using System.Globalization;

namespace webTry2.Controllers
{
    public class EncryptorSubController : DBController
    {
        public EncryptorSubController()
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

        public List<Encryptor> GetEncryptorBySN (string encSN , string tblName)
        {
            List<Encryptor> encList = new List<Encryptor>();
            // set ENUM for status
            sqlQuery = "SELECT * " +
                       "FROM " + tblName + "  WHERE "+tblName+".SN = '" + encSN + "';";   // "FROM " + tblName + " as Enc WHERE Enc.SN = '" + encSN + "';";

            dataReader = sendSqlQuery(sqlQuery);
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    Encryptor enc = new Encryptor();
                    enc.serialNumber = encSN;
                    enc.timestampAsString = dataReader.GetValue(2).ToString(); //date as string
                                                                               //enc.timestamp = DateTime.ParseExact(enc.timestampAsString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); //DateTime Obj
                    enc.ownerID = dataReader.GetValue(3).ToString();
                    enc.status = StatusNumberToString(Int16.Parse(dataReader.GetValue(4).ToString()));
                    enc.deviceLocation.locationID = Int16.Parse(dataReader.GetValue(5).ToString());
                    encList.Add(enc);
                }
            }
            else encList = null;
            if(encList != null)
            {
                foreach (var enc in encList)
                {
                    enc.deviceLocation = GetEncLocationBylocID(enc.deviceLocation.locationID.ToString());
                }
            }
            closeConnectionAndReading();
            return encList;
        }

        public List<Encryptor> EncryptorHistory(string encSN)
        {

            List<Encryptor> encHistory = GetEncryptorBySN(encSN, "EncryptorHistory");

            return encHistory;
        }

        private Location GetEncLocationBylocID(string locID)
        {
            Location loc = new Location();

            sqlQuery = "SELECT * from Locations WHERE Locations.locationID = '" + locID + "';";
            if (dataReader.IsClosed)
            {
                connectToSQL();
            }
            else
            {
                closeConnectionAndReading();
                connectToSQL();
            }
            dataReader = sendSqlQuery(sqlQuery);

            if (dataReader.HasRows)
            {
                dataReader.Read();
                loc.locationID  =   Int16.Parse(dataReader.GetValue(0).ToString());
                loc.facility    =   dataReader.GetValue(1).ToString();
                loc.building    =   dataReader.GetValue(2).ToString();
                loc.floor  =   UInt16.Parse(dataReader.GetValue(3).ToString());
                loc.room = UInt16.Parse(dataReader.GetValue(4).ToString());
                loc.longitude = Double.Parse(dataReader.GetValue(5).ToString());
                loc.latitude = Double.Parse(dataReader.GetValue(6).ToString());
            }
            else
            {
                Console.WriteLine("NO Location return - BUG!");
                loc = null;
            }
            closeConnectionAndReading();
            return loc;
        }

        public string StatusNumberToString (int encStatus)
        {
            string status = "";
            switch (encStatus)
            {
                case 1:
                    status = "in use";
                    break;
                case 2:
                    status = "destroyed";
                    break;
                case 3:
                    status = "lost";
                    break;
                case 4:
                    status = "delivered";
                    break;
                default:
                    status = "in use";
                    break;
            }
            return status;
        }
    }
}