using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webTry2.Models
{
    public class generalReport : ReportAbstract
    {
        public uint activeEncryptors { get; set; }
        public uint notActiveEncryptors { get; set; }
        public uint LostEncryptors { get; set; }
        public uint destroyedEncryptors { get; set; }
        public uint deliveredEncryptors { get; set; }
    }
}