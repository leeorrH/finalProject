using Microsoft.VisualStudio.TestTools.UnitTesting;
using webTry2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webTry2.Models;

namespace webTry2.Controllers.Tests
{
    [TestClass()]
    public class EncryptorSubControllerTests
    {
        EncryptorSubController EncryptorSubCtrl = new EncryptorSubController();
        [TestMethod()]
        public void EncryptorSubControllerTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void loadEncryptorByUserTest()
        {
            string existingUser = "10000";
            List<Encryptor> result = EncryptorSubCtrl.loadEncryptorByUser(existingUser);
            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            foreach(Encryptor enc in result)
            {
                Assert.IsTrue(enc.ownerID.Equals(existingUser));
            }

            result.RemoveRange(0, result.Count);
            EncryptorSubCtrl.connectToSQL();

            string unExistingUser = "10008";
            result = EncryptorSubCtrl.loadEncryptorByUser(unExistingUser);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 0);

        }

        [TestMethod()]
        public void GetEncryptorBySNTest()
        {
            string existingEnc = "201";
            string tblName = "Encryptors";

            //EncryptorSubCtrl.connectToSQL();

            List<Encryptor> result = EncryptorSubCtrl.GetEncryptorBySN(existingEnc,tblName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            foreach (Encryptor enc in result)
            {
                Assert.IsTrue(enc.serialNumber.Equals(existingEnc));
            }

            result.RemoveRange(0, result.Count);
            EncryptorSubCtrl.connectToSQL();

            string unExistingEnc = "10008";
            result = EncryptorSubCtrl.GetEncryptorBySN(unExistingEnc, tblName);
            Assert.IsNull(result);

            EncryptorSubCtrl.connectToSQL();

            tblName = "EncryptorHistory";
            result = EncryptorSubCtrl.GetEncryptorBySN(existingEnc, tblName);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            foreach (Encryptor enc in result)
            {
                Assert.IsTrue(enc.serialNumber.Equals(existingEnc));
            }

            result.RemoveRange(0, result.Count);
            EncryptorSubCtrl.connectToSQL();
            result = EncryptorSubCtrl.GetEncryptorBySN(unExistingEnc, tblName);
            Assert.IsNull(result);

        }

        [TestMethod()]
        public void EncryptorHistoryTest()
        {
            string existingEnc = "201";
            

            //EncryptorSubCtrl.connectToSQL();

            List<Encryptor> result = EncryptorSubCtrl.EncryptorHistory(existingEnc);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            foreach (Encryptor enc in result)
            {
                Assert.IsTrue(enc.serialNumber.Equals(existingEnc));
            }

            result.RemoveRange(0, result.Count);
            EncryptorSubCtrl.connectToSQL();

            string unExistingEnc = "10008";
            result = EncryptorSubCtrl.EncryptorHistory(unExistingEnc);
            Assert.IsNull(result);

        }

        [TestMethod()]
        public void StatusNumberToStringTest()
        {
            string expectedStatus = "in use";
            string result = EncryptorSubCtrl.StatusNumberToString(1);
            Assert.IsNotNull(result);
            Assert.IsTrue(expectedStatus == result);

             expectedStatus = "in use";
             result = EncryptorSubCtrl.StatusNumberToString(1);
            Assert.IsNotNull(result);
            Assert.IsTrue(expectedStatus == result);

             expectedStatus = "destroyed";
             result = EncryptorSubCtrl.StatusNumberToString(2);
            Assert.IsNotNull(result);
            Assert.IsTrue(expectedStatus == result);

             expectedStatus = "lost";
             result = EncryptorSubCtrl.StatusNumberToString(3);
            Assert.IsNotNull(result);
            Assert.IsTrue(expectedStatus == result);

             expectedStatus = "delivered";
             result = EncryptorSubCtrl.StatusNumberToString(4);
            Assert.IsNotNull(result);
            Assert.IsTrue(expectedStatus == result);

            expectedStatus = "in use";
            result = EncryptorSubCtrl.StatusNumberToString(6); // check default
            Assert.IsNotNull(result);
            Assert.IsTrue(expectedStatus == result);
        }

        [TestMethod()]
        public void GetEncLocationBylocIDTest()
        {
            string existingLocID = "1";
            string unExistingLocID = "255";

            Location result = EncryptorSubCtrl.GetEncLocationBylocID(existingLocID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.locationID == 1);

            result = EncryptorSubCtrl.GetEncLocationBylocID(unExistingLocID);
            Assert.IsNull(result);
        }
    }
}