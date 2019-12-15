using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models
{
    public abstract class ReportAbstract
    {
        public int reportID { get; set; }
        public String createdBy { get; set; }
        public DateTime date { get; set; }
        public String notifications { get; set; }
        public List<Encryptor> encryptorsList { get; set; } 

        public ReportAbstract()
        {
            date = DateTime.Now;
            encryptorsList = new List<Encryptor>();
        }

    }
}