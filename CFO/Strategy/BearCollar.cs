using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO.Strategy
{
    public class BearCollar : Combo
    {
        public Position PosLowerK { get; set; }
        public Position PosHigherK { get; set; }
        public Position PosSpot { get; set; }

        public Option OptLowerK { get; set; }
        public Option OptHigherK { get; set; }


        public override void TypeJudge()
        {
            this.LoseType = LoseType.LimitedLose;
            this.ProfitType = ProfitType.LimitedProfit;
        }

        public BearCollar(Position LegSpot,Position LegHigherK,Position LegLowerK)
        {
            PosSpot = LegSpot;
            PosLowerK = LegLowerK;
            PosHigherK = LegHigherK;

            Legs.Add(LegSpot);
            Legs.Add(LegHigherK);
            Legs.Add(LegLowerK);

            if (VaildateSameSymbol() && VaildateBearCollar())
                IsVaildated = true;
            else
                IsVaildated = false;
        }

        private bool VaildateBearCollar()
        {
            IsVaildated = false;

            //BearCollar 必须是三腿的
            if(Legs.Count!=3)
                throw new VaildateException("BearCollar 必须是三腿的");

            var OptionLegsCount = Legs.Count((leg) => leg.Asset.GetType() == typeof(Option));
            var SpotLegsCount = Legs.Count((leg) => leg.Asset.GetType() == typeof(STK));

            if (!(OptionLegsCount == 2 && SpotLegsCount == 1))
                throw new VaildateException("BearCollar 必须有两腿期权和一腿现货,那才叫BearCollar啊！");

            //期权腿的集合
            var OptionLegs = new List<Position>();

            //归置出现货和期权头寸
            Legs.ForEach((leg) =>
            {
                if (leg.Asset.GetType() == typeof(Option))
                    OptionLegs.Add(leg);
                else
                {
                    if (leg.Quantity >= 0)
                        throw new VaildateException("BearCollar 必须有现货头寸必须是做空的");
                    else
                        PosSpot = leg;
                }
            });

            Option opt1 = (Option)OptionLegs[0].Asset;
            Option opt2 = (Option)OptionLegs[1].Asset;

            if (opt1.StrikePrice == opt2.StrikePrice)
                throw new VaildateException("期权腿的执行价不能相同");

             //必须是异常权与同到期日的
            if ((opt1.Right == opt2.Right) || (opt1.ExpiryDate != opt2.ExpiryDate))
                throw new VaildateException("BearCollar 两腿必须异权且同到期日");

            //必须是高执行价的Call做多，低执行价的Put做空
            if (opt1.StrikePrice > opt2.StrikePrice)
            {
                if ((opt1.Right == OptionRight.CALL && OptionLegs[0].Quantity>0) &&
                    (opt2.Right == OptionRight.PUT) && OptionLegs[1].Quantity<0)
                {
                    OptHigherK = opt1;
                    OptLowerK = opt2;

                    PosHigherK = OptionLegs[0];
                    PosLowerK = OptionLegs[1];
                }
                else
                    throw new VaildateException("BearCollar 组成异常");
            }
            else
            {
                if ((opt2.Right == OptionRight.CALL && OptionLegs[1].Quantity > 0) &&
                    (opt1.Right == OptionRight.PUT) && OptionLegs[0].Quantity < 0)
                {
                    OptHigherK = opt2;
                    OptLowerK = opt1;

                    PosHigherK = OptionLegs[1];
                    PosLowerK = OptionLegs[0];
                }
                else
                    throw new VaildateException("BearCollar 组成异常");
            }

            return true;
        }

        public override double CalculateMaxProfit()
        {
            MaxProfit = Legs.Sum((leg) => leg.CalculateFinalProfit(0));

            if (MaxProfit < 0)
                throw new Exception("Bear Spread 的最大收益不应小于0");

            return MaxProfit;
        }

        public override double CalculateMaxLose()
        {
            MaxLose = Legs.Sum((leg) => leg.CalculateFinalProfit(OptHigherK.StrikePrice));

            if (MaxLose > 0)
                throw new Exception("Bear Spread 的最大损失不应大于0");

            return MaxLose;
        }
    }
}
