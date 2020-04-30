using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models
{
    public class Location
    {
        public int locationID { get; set; }
        public string facility { get; set; }
        public string building { get; set; }
        public int floor { get; set; }
        public uint room { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}