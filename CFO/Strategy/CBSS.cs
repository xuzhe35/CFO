using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO.Strategy
{
    /// <summary>
    /// CBSS 为 Call Back Spread with spot的简称
    /// Spot 现货做空，配合一个Call BackSpread。保证无限上涨端和下跌端都是安全的
    /// </summary>
    public class CBSS : Combo
    {
        public Position PosLowerK { get; set; }
        public Position PosHigherK { get; set; }
        public Position PosSpot { get; set; }

        public Option OptLowerK { get; set; }
        public Option OptHigherK { get; set; }

        public CBSS(Position LegSpot, Position LegHigherK, Position LegLowerK)
        {
            PosSpot = LegSpot;
            PosLowerK = LegLowerK;
            PosHigherK = LegHigherK;

            Legs.Add(LegSpot);
            Legs.Add(LegHigherK);
            Legs.Add(LegLowerK);

            this.LoseType = LoseType.LimitedLose;
            this.ProfitType = ProfitType.UnLimitedProfit;

            if (VaildateSameSymbol() && VaildateCBSS())
                IsVaildated = true;
            else
                IsVaildated = false;
        }

        public CBSS()
        {
            Strategy = ComboStrategy.CBSS;

            if (VaildateSameSymbol() && VaildateCBSS())
                IsVaildated = true;
            else
                IsVaildated = false;
        }

        public bool IsInvolved(string ID)
        {
            if (OptLowerK.ID == ID || OptHigherK.ID == ID || PosSpot.ID == ID)
            {
                return true;
            }

            return false;
        }

        public bool VaildateCBSS()
        {
            IsVaildated = false;

            //BearCollar 必须是三腿的
            if (Legs.Count != 3)
                throw new VaildateException("CBSS 必须是三腿的");

            var OptionLegsCount = Legs.Count((leg) => leg.Asset.GetType() == typeof(Option));
            var SpotLegsCount = Legs.Count((leg) => leg.Asset.GetType() == typeof(STK));

            if (!(OptionLegsCount == 2 && SpotLegsCount == 1))
                throw new VaildateException("CBSS 必须有两腿期权和一腿现货,那才叫BearCollar啊！");

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
                        throw new VaildateException("CBSS 必须有现货头寸且必须是做空的");
                    else
                        PosSpot = leg;
                }
            });

            Option opt1 = (Option)OptionLegs[0].Asset;
            Option opt2 = (Option)OptionLegs[1].Asset;

            if (opt1.StrikePrice == opt2.StrikePrice)
                throw new VaildateException("期权腿的执行价不能相同");

            //必须是异常权与同到期日的
            if ((opt1.Right== OptionRight.PUT || opt2.Right== OptionRight.PUT) || (opt1.ExpiryDate != opt2.ExpiryDate))
                throw new VaildateException("CBSS 两腿期权必须都是看涨期权且同到期日");

            if (opt1.StrikePrice > opt2.StrikePrice)
            {
                OptHigherK = opt1;
                OptLowerK = opt2;

                PosHigherK = OptionLegs[0];
                PosLowerK = OptionLegs[1];
            }
            else
            {
                OptHigherK = opt2;
                OptLowerK = opt1;

                PosHigherK = OptionLegs[1];
                PosLowerK = OptionLegs[0];
            }

            //高执行价价买低执行价卖 Higher(k) Buy Lower(k) Sell
            bool HBLS = PosHigherK.Quantity > 0 && PosLowerK.Quantity < 0;
            if (!HBLS)
                throw new VaildateException("Call BackSpread 必须保证更高执行价是做多，低执行价是做空的");

            //高执行价看涨期权的张数必须比较低执行价的要高。
            if (!(PosHigherK.Quantity > Math.Abs(PosLowerK.Quantity)))
                throw new VaildateException("Call BackSpread 必须保证更高执行价的期权张数更多");

            //现货的持仓量必然是个负数，先取绝对值
            bool Safe = Math.Abs(PosSpot.Quantity) <= PosHigherK.Quantity - PosLowerK.Quantity;

            if (!Safe)
                throw new VaildateException("CBSS 必须保障两边的安全");

            return false;
        }

        public override double CalculateMaxLose()
        {
            MaxLose = Legs.Sum((leg) => leg.CalculateFinalProfit(OptHigherK.StrikePrice));

            if (MaxLose > 0)
                throw new Exception("Call Back Spread 的最大损失不应大于0");

            return MaxLose;
        }
    }
}
