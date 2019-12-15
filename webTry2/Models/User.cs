using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webTry2.Models
{
    public class User
    {
        [Key]
        public string userName { get; set; }
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string permission { get; set; }
        public string unit { get; set; }
    }
}