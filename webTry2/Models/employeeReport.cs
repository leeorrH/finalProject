using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models
{
    public class employeeReport : ReportAbstract
    {
        public string encryptorID;
        public String reference ;
        public bool approvementStatus;
        public string encNewStatus;

        public employeeReport() {
   //       encryptor= new Encryptor();
        }

   /*     public employeeReport(string owner, DateTime reportDate, string encSerialNum, string encryptorStatus)
        {
            reportOwner = owner;
            date = reportDate;
            encryptorID = encSerialNum;
            encNewStatus = encryptorStatus;
        }*/
    }

}