using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models.requests
{
    public class ChangingOwnerReq
    {
        public string requestOption { get; set; }
        public string newEmpID { get; set; }
        public string siteName { get; set; }
        public string buldingName { get; set; }
        public int floorNum { get; set; }
        public int roomNum { get; set; }
    }
}