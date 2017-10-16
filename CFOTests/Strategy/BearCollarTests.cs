using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFO.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO.Strategy.Tests
{
    [TestClass()]
    public class BearCollarTests
    {
        [TestMethod()]
        public void CalculateMaxProfitTest()
        {
            Option opt1 = new Option("VXX", OptionRight.PUT, 33, "20171117");
            Option opt2 = new Option("VXX", OptionRight.CALL, 37, "20171117");
            STK spot = new STK("VXX");

            Position ShortPut = new Position()
            {
                Asset = opt1,
                AvgCost = 1.359,
                Quantity = -1500
            };

            Position BuyCall = new Position()
            {
                Asset = opt2,
                AvgCost = 2.061,
                Quantity = 1500
            };

            Position ShortSpot = new Position()
            {
                Asset = spot,
                AvgCost = 35,
                Quantity = -1500
            };

            BearCollar bear_collar = new BearCollar(ShortSpot, BuyCall, ShortPut);

            if (bear_collar.IsVaildated)
            {
                var max_profit = bear_collar.CalculateMaxProfit();
                var max_lose = bear_collar.CalculateMaxLose();


                var ratio = bear_collar.CalculateRiskRatio();

                Assert.IsTrue(max_profit > 0);
            }
        }
    }
}