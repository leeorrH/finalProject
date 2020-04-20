using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Mvc;
using WEB_project.Controllers;
using webTry2.Models;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using webTry2.Models.requests;

namespace webTry2.Controllers
{
    public class AdminController : EmployeeController
    {
        private List<Encryptor> userEncryptors;

        public AdminController()
        {
        }

        // GET: Admin
        public ActionResult adminPage()
        {
            return View();
        }

        //loadAllEncryptors - function that get all encryptors from SQL DB
        [HttpPost]
        public override ActionResult loadEmployeeEncryptors(string userName)
        {
            userEncryptors = new List<Encryptor>();
            //connection to DB 
            connectToSQL();

            //query for getting ALL encryptors and thier location from the user
            sqlQuery = "select enc.SN, enc.ownerID, CONVERT(varchar,enc.timeStamp,20) ,stat.statusName , loc.*,CONVERT(varchar,enc.lastSeenDate,11) " +
                       " from Encryptors as enc, Locations as loc , Status as stat " +
                        " where enc.locationID = loc.locationID and enc.status = stat.statusID";

            command = new SqlCommand(sqlQuery, connectionToSql);

            dataReader = command.ExecuteReader();

            //adding data to classes by query
            while (dataReader.Read())
            {
                Encryptor temp = new Encryptor();
                Location loc = new Location();
                loc.locationID = Int16.Parse(dataReader.GetValue(4).ToString());// location id not exist=> the encryptor destroyed
                if (!userName.Equals("withDestroyed") && loc.locationID==0) continue;
                /*              adding data to Encryptor                   */
               
                temp.serialNumber = dataReader.GetValue(0).ToString(); //Encryptor SN
                temp.ownerID = dataReader.GetValue(1).ToString(); // owner ID - employee user name
                //setting date 
                temp.timestampAsString = dataReader.GetValue(2).ToString(); //date as string
                temp.timestamp = DateTime.ParseExact(temp.timestampAsString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); // date as an object
                temp.status = dataReader.GetValue(3).ToString(); // 
                temp.lastSeenDate= dataReader.GetValue(11).ToString();//last seen Date
                //adding data to location

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


            closeConnectionAndReading();
            return Json(userEncryptors, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult exportEncExl()
        {
            getEmployeeList();
            loadEmployeeEncryptors("withDestroyed");
            Excel.Application xlApp = new
          Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                System.Console.WriteLine("Excel is not properly installed!!");
                return Json("Excel is not properly installed!!", JsonRequestBehavior.AllowGet);
            }
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Cells[1, 1] = "Serial Number";
            xlWorkSheet.Cells[1, 2] = "timeStamp";
            xlWorkSheet.Cells[1, 3] = "status";
            xlWorkSheet.Cells[1, 4] = "last seen Date";
            //location
            xlWorkSheet.Cells[1, 5] = "facility";
            xlWorkSheet.Cells[1, 6] = "building";
            xlWorkSheet.Cells[1, 7] = "floor";
            xlWorkSheet.Cells[1, 8] = "room";
            //owner
            xlWorkSheet.Cells[1, 9] = "owner userName";
            xlWorkSheet.Cells[1, 10] = "owner Name";

            for (int i=0;i< userEncryptors.Count;i++)
            {
                xlWorkSheet.Cells[i + 2, 1] = userEncryptors[i].serialNumber;
                xlWorkSheet.Cells[i + 2, 2] = userEncryptors[i].timestampAsString;
                xlWorkSheet.Cells[i + 2, 3] = userEncryptors[i].status;
                xlWorkSheet.Cells[i + 2, 4] = userEncryptors[i].lastSeenDate; ;
                //location
                xlWorkSheet.Cells[i + 2, 5] = userEncryptors[i].deviceLocation.facility;
                xlWorkSheet.Cells[i + 2, 6] = userEncryptors[i].deviceLocation.building;
                xlWorkSheet.Cells[i + 2, 7] = userEncryptors[i].deviceLocation.floor;
                xlWorkSheet.Cells[i + 2, 8] = userEncryptors[i].deviceLocation.room;
                //owner
                User owner = employees.Find(emp => emp.userName== userEncryptors[i].ownerID);
                xlWorkSheet.Cells[i + 2, 9] = userEncryptors[i].ownerID;
                xlWorkSheet.Cells[i + 2, 10] = owner.lastName +" " + owner.firstName;
            }

            xlWorkBook.SaveAs("C:\\Users\\leeorr\\Downloads\\csharp-Excel.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);


            System.Console.WriteLine("encryptor Exl Report Created !!");
            return Json("encryptor Exl Report Created !", JsonRequestBehavior.AllowGet);/// ?
        }

        [HttpPost]
        public string setReportStatus(EmpReport reportToUpdate )
        {
            string result = "";
            //reportToUpdate.managerInCharge = employees.Find(emp => emp.userName == reportToUpdate.managerInCharge.userName);
            sqlQuery = "UPDATE EmployeeReport " +
                       "SET approvementStatus = '" + reportToUpdate.approvementStatus + "' ," +
                       "managerInCharge = '" + reportToUpdate.managerInCharge.userName + "' " +
                       "WHERE reportID = '" + reportToUpdate.reportID + "' "; 
                             

            if (!dataReader.IsClosed && dataReader.HasRows) closeConnectionAndReading();
            if (dataReader.IsClosed)
                connectToSQL();

            var rowsEffected = sqlIUDoperation(sqlQuery);
            if(rowsEffected == 1)
            {
                //call orit function
                result = reportToUpdate.approvementStatus;
            }
            else
            {
                throw new Exception("No records was effected! ");
            }
            if (!dataReader.IsClosed) closeConnectionAndReading();
            // TBD - need to change Empreport OBJ and DB Table ! there are 3 cases "waiting", "approved", "denied" - so UI (getReports) and BE ( ) need to be changed!  
            return result;
        }
    }

}