using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO.Strategy
{
    public class RiskReversal : Combo
    {
        public Position PosLowerK { get; set; }
        public Position PosHigherK { get; set; }

        public Option OptLowerK { get; set; }
        public Option OptHigherK { get; set; }

        public RiskReversal()
        {
            LoseType = LoseType.LimitedLose;
            ProfitType = ProfitType.UnLimitedProfit;
        }

        public bool IsInvolved(string ID)
        {
            if (OptLowerK.ID == ID || OptHigherK.ID == ID)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Risk reversal 组合的验证，必须是两腿的，不同执行价的，同权同到期日的，张数必须是相同的
        /// </summary>
        /// <returns></returns>
        private bool VaildateRiskReversal()
        {
            IsVaildated = false;

            //期权的腿必须是两腿
            if (Legs.Count != 2)
                throw new VaildateException("Risk reversal 必须是两腿的");

            //左腿和右腿
            Option opt1 = (Option)Legs[0].Asset;
            Option opt2 = (Option)Legs[1].Asset;

            //执行价不能相同
            if (opt1.StrikePrice == opt2.StrikePrice)
                throw new VaildateException("Risk reversal 两腿的执行价不能一致");

            //必须是同权与同到期日的
            if ((opt1.Right != opt2.Right) || (opt1.ExpiryDate != opt2.ExpiryDate))
                throw new VaildateException("Risk reversal 两腿必须同权且同到期日");

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

            //一定是低执行价的期权卖，高执行价的期权买；否则就不是Risk reversal
            if (!(PosHigherK.Quantity > 0 && PosLowerK.Quantity < 0))
                throw new VaildateException("Risk reversal 必须是低执行价的卖出高执行价买入");

            //张数必须相等
            if (Math.Abs(PosHigherK.Quantity) != Math.Abs(PosLowerK.Quantity))
                throw new VaildateException("Risk reversal 两腿的张数必须配平");

            OptLowerK = (Option)PosLowerK.Asset;
            OptHigherK = (Option)PosHigherK.Asset;

            return true;
        }
    }
}
