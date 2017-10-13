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
    public class ComboTests
    {
        [TestMethod()]
        public void CalculateFPLTest()
        {
            Option opt1 = new Option(OptionRight.CALL, 38, "20171010");
            Option opt2 = new Option(OptionRight.CALL, 41, "20171010");

            Price opt1_price = new Price()
            {
                Ask = 1.85,
                Bid = 1.84
            };

            opt1.CurrentPrice = opt1_price;

            Price opt2_price = new Price()
            {
                Ask = 1.35,
                Bid = 1.34
            };

            opt2.CurrentPrice = opt2_price;

            Position pos1 = new Position()
            {
                Asset = opt1,
                AvgCost = 3.598,
                Quantity = -1500
            };

            Position pos2 = new Position()
            {
                Asset = opt2,
                AvgCost = 2.718,
                Quantity = 1500
            };

            Combo bear_spread = new Combo();
            bear_spread.MemberPos.Add(pos1);
            bear_spread.MemberPos.Add(pos2);

            var pl = bear_spread.CalculateFPL();

            Assert.AreEqual(pl, bear_spread.CalculateFPL());
        }
    }
}