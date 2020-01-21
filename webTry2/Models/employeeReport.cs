using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models
{
    public class employeeReport : ReportAbstract
    {
        public Encryptor encryptor= new Encryptor();
        public String reference { get; set; } = null;
        public bool approvementStatus { get; set; } = false;


        public employeeReport() {
          
        }

        public employeeReport(string owner, DateTime reportDate, string encSerialNum, string encryptorStatus)
        {
            reportOwner = owner;
            date = reportDate;
            encryptor.serialNumber = encSerialNum;
            encryptor.status = encryptorStatus;
        }
    }

}