using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO
{
    public class Combo
    {
        public bool IsVaildated { get; set; }
        public List<Position> Legs = new List<Position>();

        public string Symbol { get; set; }
        public ComboStrategy Strategy { get; set; }
        public double FloatingPL { get; set; }
        public Greeks ComboGreeks { get; set; }

        public ProfitType ProfitType { get; set; }
        public LoseType LoseType { get; set; }

        public double MaxProfit { get; set; }
        public double MaxLose { get; set; }

        /// <summary>
        /// 验证一个combo里的所有成员是否都是一个标的物
        /// </summary>
        /// <returns></returns>
        public bool VaildateSameSymbol()
        {
            if (Legs.Count > 0)
            {
                var symbol = Legs[0].Asset.Symbol;
                //如果成员里但凡有一个不是相同的symbol，则返回验证失败
                if (Legs.Exists((mp) => mp.Asset.Symbol != symbol))
                {
                    IsVaildated = false;
                    return false;
                }
                else
                    return true;
            }
            else
            {
                IsVaildated = false;
                return false;
            }
        }

        public virtual void TypeJudge()
        {

        }

        public virtual double CalculateMaxProfit()
        {
            return 0;
        }

        public virtual double CalculateMaxLose()
        {
            return 0;
        }

        /// <summary>
        /// 计算整个组合的浮动盈亏
        /// </summary>
        /// <returns></returns>
        public double CalculateFPL()
        {
            Legs.ForEach((p) => p.CalculateFPL());
            FloatingPL = Legs.Sum((p) => p.FloatingPL);

            return FloatingPL;
        }

        /// <summary>
        /// 计算合并的希腊值，只有同名的资产该方法才有效
        /// </summary>
        public void CalculateGreeks()
        {
            if (!VaildateSameSymbol())
                throw new Exception("非同一个标的物的希腊值不能叠加");
             
            double SumDelta = 0;
            double SumGamma = 0;
            double SumVega = 0;
            double SumTheta = 0;

            foreach(Position pos in Legs)
            {
                if (pos.Asset.GetType() == typeof(STK))
                {
                    SumDelta += 1 * pos.Quantity;
                }
                else if(pos.Asset.GetType() == typeof(Option))
                {
                    Option opt = (Option)pos.Asset;
                    SumDelta += opt.Greeks.Delta * pos.Quantity;
                    SumGamma += opt.Greeks.Gamma * pos.Quantity;
                    SumVega += opt.Greeks.Vega * pos.Quantity;
                    SumTheta += opt.Greeks.Theta * pos.Quantity;
                }
            }

            ComboGreeks.Delta = SumDelta;
            ComboGreeks.Gamma = SumGamma;
            ComboGreeks.Vega = SumVega;
            ComboGreeks.Theta = SumTheta;
        }

        /// <summary>
        /// 计算整个组合的点差和
        /// </summary>
        /// <returns></returns>
        public double SumSpread()
        {
            //能一句话解决的坚决不换行
            return Legs.Sum((leg) => (leg.Asset.CurrentPrice.Ask - leg.Asset.CurrentPrice.Bid) * Math.Abs(leg.Quantity));
        }
    }
}
