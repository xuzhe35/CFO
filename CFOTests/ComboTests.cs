using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFO.Strategy;

namespace CFO.Tests
{
    [TestClass()]
    public class ComboTests
    {
        [TestMethod()]
        public void CalculateFPLTest()
        {
            Option opt1 = new Option("VXX", OptionRight.CALL, 38, "20171010");
            Option opt2 = new Option("VXX", OptionRight.CALL, 41, "20171010");

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
            bear_spread.Legs.Add(pos1);
            bear_spread.Legs.Add(pos2);

            var pl = bear_spread.CalculateFPL();

            Assert.AreEqual(pl, bear_spread.CalculateFPL());
        }

        [TestMethod()]
        public void CalculateMaxProfitTest()
        {
            Option opt1 = new Option("VXX", OptionRight.CALL, 38, "20171010");
            Option opt2 = new Option("VXX", OptionRight.CALL, 41, "20171010");

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

            BearSpread bear_spread = new BearSpread(pos1, pos2);

            if (bear_spread.IsVaildated)
            {
                var max_lose = bear_spread.CalculateMaxProfit();
                Assert.IsTrue(bear_spread.MaxProfit == 1320);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void CalculateMaxLoseTest()
        {
            Option opt1 = new Option("VXX", OptionRight.CALL, 38, "20171010");
            Option opt2 = new Option("VXX", OptionRight.CALL, 41, "20171010");

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

            BearSpread bear_spread = new BearSpread(pos1, pos2);

            if (bear_spread.IsVaildated)
            {
                var max_lose = bear_spread.CalculateMaxLose();
                Assert.IsTrue(bear_spread.MaxLose < -1320);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void CalculateRiskRatioTest()
        {
            Option opt1 = new Option("VXX", OptionRight.CALL, 38, "20171010");
            Option opt2 = new Option("VXX", OptionRight.CALL, 41, "20171010");

            opt1.CurrentPrice = new Price() { Ask = 1.85, Bid = 1.84 };
            opt2.CurrentPrice = new Price() { Ask = 1.35, Bid = 1.34 };

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

            BearSpread bear_spread = new BearSpread(pos1, pos2);

            if (bear_spread.IsVaildated)
            {
                var ration = bear_spread.CalculateRiskRatio();

                //Console.WriteLine(ration.ToString());
                Assert.IsTrue(ration > 2);
            }
            else
            {
                Assert.Fail("Vaildate fail");
            }
        }
    }
}