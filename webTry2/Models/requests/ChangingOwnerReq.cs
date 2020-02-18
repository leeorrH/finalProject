using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models.requests
{
    public class ChangingOwnerReq
    {
        public string requestOption { get; set; }
        public Encryptor enc { get; set; }
    }
}