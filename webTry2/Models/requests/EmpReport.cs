using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models.requests
{
    public class EmpReport : ReportAbstract
    {
        public Encryptor enc { get; set; }
        public string reference { get; set; }
        public bool approvementStatus { get; set; }
    }
}