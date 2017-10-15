using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO.Strategy
{
    public enum BearSpreadType
    {
        BearCallSpread,
        BearPutSpread
    }

    public class BearSpread : Combo
    {
        public Position PosLowerK { get; set; }
        public Position PosHigherK { get; set; }

        public Option OptLowerK { get; set; }
        public Option OptHigherK { get; set; }

        public BearSpreadType BearSpreadType { get; set; }

        /// <summary>
        /// 初始化函数
        /// </summary>
        public BearSpread()
        {
            Strategy = ComboStrategy.BearSpread;

            if (VaildateSameSymbol() && VaildateBearSpread())
            {
                IsVaildated = true;
            }
            else
            {
                IsVaildated = false;
            }
        }

        public override void TypeJudge()
        {
            this.LoseType = LoseType.LimitedLose;
            this.ProfitType = ProfitType.LimitedProfit;

            //已经验证同权的情况下，判断是用Call构成的还是用Put构成的
            if (((Option)Legs[0].Asset).Right == OptionRight.CALL)
                BearSpreadType = BearSpreadType.BearCallSpread;
            else
                BearSpreadType = BearSpreadType.BearPutSpread;
        }

        public BearSpread(Position Leg1,Position Leg2)
        {
            Legs.Add(Leg1);
            Legs.Add(Leg2);

            Strategy = ComboStrategy.BearSpread;

            if (VaildateSameSymbol() && VaildateBearSpread())
            {
                IsVaildated = true;
            }
            else
            {
                IsVaildated = false;
            }
        }

        /// <summary>
        /// BearSpread 组合的验证，必须是两腿的，不同执行价的，同权同到期日的，张数必须是相同的
        /// </summary>
        /// <returns></returns>
        private bool VaildateBearSpread()
        {
            IsVaildated = false;

            //期权的腿必须是两腿
            if (Legs.Count != 2)
                throw new VaildateException("BearSpread 必须是两腿的");

            //必须是两张期权
            if (Legs[0].GetType() != typeof(Option) || Legs[1].GetType() != typeof(Option))
            {
                throw new VaildateException("BearSpread 两腿必须都是期权");
            }

            //左腿和右腿
            Option opt1 = (Option)Legs[0].Asset;
            Option opt2 = (Option)Legs[1].Asset;

            //执行价不能相同
            if (opt1.StrikePrice == opt2.StrikePrice)
                throw new VaildateException("BearSpread 两腿的执行价不能一致");

            //必须是同权与同到期日的
            if ((opt1.Right != opt2.Right) || (opt1.ExpiryDate != opt2.ExpiryDate))
                throw new VaildateException("BearSpread 两腿必须同权且同到期日");

            TypeJudge();

            //找到高执行价和低执行价
            if (opt1.StrikePrice > opt2.StrikePrice)
            {
                PosHigherK = Legs[0];
                PosLowerK = Legs[1];
            }
            else
            {
                PosHigherK = Legs[1];
                PosLowerK = Legs[0];
            }

            //一定是低执行价的期权卖，高执行价的期权买；否则就不是BearSpread
            if (PosHigherK.Quantity > 0 && PosLowerK.Quantity < 0)
                throw new VaildateException("BearSpread 必须是低执行价的卖出高执行价买入");

            //张数必须相等
            if (Math.Asin(PosHigherK.Quantity) != Math.Abs(PosLowerK.Quantity))
                throw new VaildateException("BearSpread 两腿的张数必须配平");

            OptLowerK = (Option)PosLowerK.Asset;
            OptHigherK = (Option)PosHigherK.Asset;

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
            MaxLose = Legs.Sum((leg) => leg.CalculateFinalProfit(10000));

            if(MaxLose>0)
                throw new Exception("Bear Spread 的最大损失不应大于0");

            return MaxLose;
        }
    }
}
