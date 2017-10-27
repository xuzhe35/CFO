using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO.Tests
{
    [TestClass()]
    public class OptionTests
    {
        [TestMethod()]
        public void GenExpiryDateTest()
        {
            Option testOption = new Option("SPY", OptionRight.CALL, 254, "20171117");

            if (testOption.GenExpiryDate())
            {
                Assert.AreEqual(testOption.ExpiryDate.Year, 2017);
                Assert.AreEqual(testOption.ExpiryDate.Month, 11);
                Assert.AreEqual(testOption.ExpiryDate.Day, 17);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void GetIntrinsicValueTest()
        {
            Option testOption = new Option("SPY", OptionRight.CALL, 254, "20171117");
            Option testPutOpt = new Option("SPY", OptionRight.PUT, 253, "20171117");

            Price testFinalPrice = new Price() { Ask = 256.01, Bid = 256, Last = 256 };

            Assert.AreEqual(0, testPutOpt.GetIntrinsicValue(testFinalPrice));
            Assert.AreEqual(2, testOption.GetIntrinsicValue(testFinalPrice));
        }

        [TestMethod()]
        public void OptionTest()
        {
            Option test = new Option("OPT.VXX.C.20171117.35.5");

            Assert.AreEqual(test.StrikePrice, 35.5);
        }
    }
}