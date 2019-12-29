using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models
{
    public class employeeReport : ReportAbstract
    {
        public String reference { get; set; }
        public char approvementStatus { get; set; }

        public employeeReport(String referenceFromUser)
        {
            reference = referenceFromUser;
        }
    }

}