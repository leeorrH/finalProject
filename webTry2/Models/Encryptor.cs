﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models
{
    public class Encryptor
    {
        public string serialNumber { get; set; }
        public string timestampAsString { get; set; }
        public string ownerID { get; set; }
        public string status { get; set; }
        public Location deviceLocation { get; set; }
        public string lastReported { get; set; }
        public Encryptor(){

            deviceLocation = new Location();

        }
    }
}