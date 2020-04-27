using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using webTry2.Models;
using webTry2.Models.requests;
using System.Net;
using System.Net.Mail;

namespace webTry2.Controllers
{
    public class EmployeeReportSubController : DBController
    {
        private EncryptorSubController encSubCntrl = new EncryptorSubController();

        
        public EmployeeReportSubController() { }



        public void SendMonthlyReport(EmpReport empReport)
        {

            sqlQuery = "INSERT INTO dbo.EmployeeReport (reportType,reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus)" +
                          " VALUES('" + empReport.reportType + "', '" + empReport.reportOwner + "', '" + DateToString(empReport) + "', '" + empReport.enc.serialNumber + "', NULL, NULL, NULL,'" + empReport.notifications + "', NULL, 'waiting for approvment'); ";
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
            if (dataReader.IsClosed)
                connectToSQL();
            if (empReport.enc.deviceLocation != null)
            {
                sqlQuery = "INSERT INTO dbo.EmployeeReport (reportType,reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus) " +
                        "OUTPUT inserted.reportID " +
                        "VALUES('" + empReport.reportType + "', '" + empReport.reportOwner + "', '" + DateToString(empReport) + "', '" + empReport.enc.serialNumber + "', NULL, NULL,'" +empReport.enc.deviceLocation.locationID+ "' ,'" + empReport.notifications + "', NULL, 'waiting for approvment'); ";

                var res = sqlIUDoperation(sqlQuery); // TODO if false then ... 
                if (res == 0)
                {
                    throw new System.InvalidOperationException("operation FAILED! NO ROWS HAS BEEN EFFECTED");
                }
            }
            closeConnectionAndReading();
            return;
        }

        public void DeliverToEmpReport (EmpReport empReport)
        {
             empReport.enc.deviceLocation = SetLocationID(empReport.enc.deviceLocation);
             if(dataReader.IsClosed)
             connectToSQL();
             sqlQuery = "INSERT INTO dbo.EmployeeReport (reportType,reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus) " +
                        "OUTPUT inserted.reportID " +
                        "VALUES('" + empReport.reportType + "', '" + empReport.reportOwner + "', '" + DateToString(empReport) + "', '" + empReport.enc.serialNumber + "', '" + empReport.enc.ownerID + "', NULL,'" + empReport.enc.deviceLocation.locationID + "' ,'" + empReport.notifications + "', NULL, 'waiting for approvment'); ";
            var res = sqlIUDoperation(sqlQuery); // TODO if false then ... 
            if (res == 0)
            {
                throw new System.InvalidOperationException("operation FAILED! NO ROWS HAS BEEN EFFECTED");
            }
            return;
        }

 //orit
        public void changingStatus(EmpReport empReport)
        {
            int statusNumber = -1;
            switch (empReport.enc.status)
            {
                case "destroyed":
                    statusNumber = 2;
                    break;
                case "lost":
                    statusNumber = 3;
                    break;
                case "delivered":
                    statusNumber = 4;
                    break;
                default:
                    System.Console.WriteLine(" status was not valid and set as -in use- ! ");
                    statusNumber = 1;
                    break;
            }
            closeConnectionAndReading();
            connectToSQL();
            sqlQuery = "INSERT INTO dbo.EmployeeReport (reportType,reportOwner, date,encSN, ownerID,encStatus,location,notifications,reference,approvementStatus)" +
                     " VALUES('" + empReport.reportType + "', '" + empReport.reportOwner + "', '" + DateToString(empReport) + "', '" + empReport.enc.serialNumber + "', NULL, " + statusNumber + ", NULL,'" + empReport.notifications + "','"+empReport.reference+ "', 'waiting for approvment'); ";
            var res = sqlIUDoperation(sqlQuery);
            if (res == 0)
            {
                throw new System.InvalidOperationException("operation FAILED! NO ROWS HAS BEEN EFFECTED");
            }
            if (!dataReader.IsClosed)
            {
                closeConnectionAndReading();
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

        public List<EmpReport> GetEmpReports (string userName, List<User> employees)
        {
            List<EmpReport> reports = new List<EmpReport>();
           
            sqlQuery = "SELECT * " +
                       "FROM EmployeeReport AS ER " +
                       "WHERE ER.reportOwner = '" + userName + "' " +
                       "AND MONTH(ER.date) = MONTH(GETDATE()) " +
                       "AND YEAR(ER.date) = YEAR(GETDATE());"; // this query return only reports that relevant to current month

            if (dataReader.IsClosed) connectToSQL();

            dataReader = sendSqlQuery(sqlQuery);

            reports = GetEmpReportsList(dataReader,employees);

            closeConnectionAndReading();

            if (reports.Count > 0) return (reports);
            else
            {
                return null;
            }
        }
        
        public List<EmpReport> GetAllReports(List<User> employees)
        {
            List<EmpReport> reports = null;

            sqlQuery = "SELECT * FROM EmployeeReport AS ER "+
                       "WHERE MONTH(ER.date) = MONTH(GETDATE()) " +
                       "AND YEAR(ER.date) = YEAR(GETDATE());";
            if (!dataReader.IsClosed && dataReader.HasRows) closeConnectionAndReading();
            if (dataReader.IsClosed)
                connectToSQL();

            dataReader = sendSqlQuery(sqlQuery);

            reports = GetEmpReportsList(dataReader,employees);

            //closeConnectionAndReading();

            if (reports.Count > 0) return (reports);
            else
            {
                return null;
            }
        }
       

        public string DateToString(EmpReport rep)
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
            if (dataReader.IsClosed)
            {
                connectToSQL();
            }
            dataReader = sendSqlQuery(sqlQuery);

            if (dataReader.Read())
            {
                deviceLocation.locationID = Int32.Parse(dataReader.GetValue(0).ToString());
                closeConnectionAndReading();
            }
            else return null;
       
            return deviceLocation;
        }

        private List<EmpReport> GetEmpReportsList (SqlDataReader dataReader, List<User> employees)
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
                        temp.reference = dataReader.GetValue(9).ToString();  
                        break;
                    default:
                        break;
                }

                if (!dataReader.GetValue(8).Equals(null))
                {
                    temp.notifications = dataReader.GetValue(8).ToString(); // can be null 
                }
               
                temp.approvementStatus = dataReader.GetValue(10).ToString();
                 
                if (!String.IsNullOrEmpty(dataReader.GetValue(11).ToString())) // check if manager in charge is exist
                {
                    string managerUserName = dataReader.GetValue(11).ToString();
                    temp.managerInCharge = employees.Find(emp => emp.userName == managerUserName);
                }
                reports.Add(temp);
            }
            
            return reports;
        }

        public void Sending_mail(User previousOwner, User newOwner, EmpReport empRep) // prevEncOwner - previous encryptor owner.
        {
            MailAddress to = new MailAddress(newOwner.email); // swap adresses for sending from the manager to employee
            MailAddress from = new MailAddress("leeorrh2@gmail.com");
            List<Encryptor>  result = encSubCntrl.GetEncryptorBySN(empRep.enc.serialNumber, "Encryptors");
            Encryptor currentEncryptorData = result[0];
            bool differentLocation = empRep.enc.deviceLocation.locationID == currentEncryptorData.deviceLocation.locationID ? false : true; //checking if there is a difference between enc location before and affter the ownership change

            string locationText;
            if (differentLocation)
            {
                locationText = "location:\n" +
                               "Facilitiy: " + empRep.enc.deviceLocation.facility + "\n" +
                               "Building: " + empRep.enc.deviceLocation.building + "\n" +
                               "Floor: " + empRep.enc.deviceLocation.floor + "\n" +
                               "Room: " + empRep.enc.deviceLocation.room ;
            }
            else locationText = "NO location change";


            MailMessage message = new MailMessage(from, to);
            message.Subject = "Changing encryptor ownership";
            message.Body = "Hello " + newOwner.firstName + " " + newOwner.lastName + "\n" +
                           "please pay attention that " + previousOwner.firstName + " " + previousOwner.lastName + " with employee number: " + previousOwner.userName + "\n" +
                           "want to pass his encryptor ownership to you\n" +
                           "Encryptor details:\n" +
                           "Encryptor SN: " + empRep.enc.serialNumber + "\n" +
                           "location:\n" +
                           "Facilitiy: " + currentEncryptorData.deviceLocation.facility + "\n" +
                           "Building: " + currentEncryptorData.deviceLocation.building + "\n" +
                           "Floor: " + currentEncryptorData.deviceLocation.floor + "\n" +
                           "Room: " + currentEncryptorData.deviceLocation.room + "\n\n" +
                           "If the report will approved by the manager the encryptor will move to: \n" + locationText + "\n\n" +
                           "If there are any questions or you do not agree with that ownership changing please contant your manager or with the report owner \n" +
                           "using phone: " + previousOwner.phoneNumber + " or Email: " + previousOwner.email + "\n" +
                           "Thank in advance.";

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("leeorrh2@gmail.com", "leeorr1992"),
                EnableSsl = true
            };
            // code in brackets above needed if authentication required 

            try
            {
                client.Send(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}

/*private string GetEmployeeMailAddress(string empUserName)
        {
            connectToSQL();
            sqlQuery = "SELECT Users.email " +
                       "FROM Users WHERE Users.userName = '" + empUserName + "';";
            dataReader = sendSqlQuery(sqlQuery);
            string empMail;
            if (dataReader.Read())
            {
                empMail = dataReader.GetValue(0).ToString();
            }
            else
            {
                //add throw event
                Console.WriteLine("no email return - BUG!");
                empMail = null;
            }
            closeConnectionAndReading();
            return empMail;
        }*/
