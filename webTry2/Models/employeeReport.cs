using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models
{
    public class employeeReport : ReportAbstract
    {
        public string encryptorID { get; set; }
        public string reference { get; set; }
        public bool approvementStatus { get; set; }
        public string encNewStatus { get; set; }
        public string managerInCharge { get; set; }

    }

}