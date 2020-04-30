using Microsoft.VisualStudio.TestTools.UnitTesting;
using webTry2.Models;

namespace webTry2.Controllers.Tests
{
    [TestClass()]
    public class HomeControllerTests
    {
        HomeController hmCtrl = new HomeController();
        [TestMethod()]
        public void HomeControllerTest()
        {
            //hmCtrl.command = new SqlCommand();
            //Assert.IsInstanceOfType(hmCtrl.command, typeof(SqlCommand) );

            Assert.IsNull(hmCtrl.command);
            Assert.IsNull(hmCtrl.sqlQuery);
        }

        [TestMethod()]
        public void IndexTest()
        {
            //
        }

        [TestMethod()]
        public void LoginCheckTest()
        {
            User user = new User();
            user.userName = "10000";
            user.password = "123";
            var res = hmCtrl.LoginCheck(user);

            Assert.IsNotNull(res);
            User returnedUser = (User)res.Data;
            Assert.AreEqual("employee",returnedUser.permission);
            Assert.AreEqual("10000", returnedUser.userName);

            user.userName = "10000";
            user.password = "";

            res = hmCtrl.LoginCheck(user);
            Assert.IsNull(res);

            user.userName = "10002";
            user.password = "123";

            res = hmCtrl.LoginCheck(user);

            Assert.IsNotNull(res);
            returnedUser = (User)res.Data;
            Assert.AreEqual("manager", returnedUser.permission);
            Assert.AreEqual("10002", returnedUser.userName);
        }

        [TestMethod()]
        public void userRegistrationTest()
        {
            //Assert.Fail();
        }
    }
}