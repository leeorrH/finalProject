﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models.requests
{
    public class EmpReport : ReportAbstract
    {
        public EmpReport()
        {
            enc = new Encryptor();
        }

        public Encryptor enc { get; set; }
        public string reference { get; set; }
        public string approvementStatus { get; set; }
        public User managerInCharge { get; set; }
    }
}