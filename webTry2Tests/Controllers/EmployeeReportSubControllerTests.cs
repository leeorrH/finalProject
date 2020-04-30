using Microsoft.VisualStudio.TestTools.UnitTesting;
using webTry2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEB_project.Controllers;
using webTry2.Models.requests;

namespace webTry2.Controllers.Tests
{
    [TestClass()]
    public class EmployeeReportSubControllerTests
    {
        EmployeeController empCtrl = new EmployeeController();
        EmployeeReportSubController repSubCtrl = new EmployeeReportSubController();
        [TestMethod()]
        public void EmployeeReportSubControllerTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void SendMonthlyReportTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void ChangingEncLocationReportTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void DeliverToEmpReportTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void changingStatusTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void GetEmpReportsTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void GetAllReportsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DateToStringTest()
        {
            EmpReport rep = new EmpReport();
            rep.date = new DateTime(2020,8,6);

            string result = repSubCtrl.DateToString(rep);
            string expectedString = "2020-08-06 00:00:00.000";

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Equals(expectedString));

        }

        [TestMethod()]
        public void Sending_mailTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetLocationByIDTest()
        {
            Assert.Fail();
        }
    }
}