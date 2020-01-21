using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models
{
    public abstract class ReportAbstract
    {
        public int reportID { get; set; }
        public String reportOwner { get; set; }
        public DateTime date { get; set; }
        public string dateAsString;
        public String notifications { get; set; }

        public ReportAbstract()
        {
            date = DateTime.Now;
            dateAsString = date.ToString();
         
        }

    }
}