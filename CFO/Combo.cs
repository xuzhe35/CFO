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

        public double RiskRatio { get; set; }

        /// <summary>
        /// 导致需要roll的触发指标，以Underlying的价格变化为准
        /// </summary>
        public double RollTrigger_Underlying { get; set; }

        /// <summary>
        /// 导致需要roll的触发指标，以组合的Gamma变化为准
        /// </summary>
        public double RollTrigger_Gamma { get; set; }

        public TriggerType TriggerType { get; set; }

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
            if (ProfitType == ProfitType.UnLimitedProfit)
                throw new Exception("无限盈利的组合无法计算极限收益");
            return 0;
        }

        public virtual double CalculateMaxLose()
        {
            if (LoseType == LoseType.UnlimitedLose)
                throw new Exception("无限损失的组合无法计算极限损失");
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

        /// <summary>
        /// 计算组合的风险收益比=极限损失/极限盈利
        /// </summary>
        /// <returns></returns>
        public double CalculateRiskRatio()
        {
            if (ProfitType == ProfitType.UnLimitedProfit || LoseType == LoseType.UnlimitedLose)
                throw new Exception("唯独同时具有极限损失和盈利的组合才能计算风险比");
            else
            {
                CalculateMaxLose();
                CalculateMaxProfit();
            }

            RiskRatio = (Math.Abs(MaxLose) / MaxProfit);
            return RiskRatio;
        }

        /// <summary>
        /// 判断是否出发了Roll的条件
        /// </summary>
        /// <param name="Underlying">当前现货的价格</param>
        /// <returns></returns>
        public bool IsTriggered(double Underlying)
        {
            bool UnderlyingUpSideSide = Underlying > RollTrigger_Underlying;
            bool UnderlyingDownSide = Underlying < RollTrigger_Underlying;

            bool GammaUpSide = ComboGreeks.Gamma > RollTrigger_Gamma;
            bool GammaDownSide = ComboGreeks.Gamma < RollTrigger_Gamma;

            if (TriggerType == TriggerType.UnderlyingUpside && UnderlyingUpSideSide)
                return true;
            else if (TriggerType == TriggerType.UnderlyingDownsid && UnderlyingDownSide)
                return true;
            else if (TriggerType == TriggerType.GammaUpside && GammaUpSide)
                return true;
            else if (TriggerType == TriggerType.GammaDownSide && GammaDownSide)
                return true;
            else if (TriggerType == TriggerType.BothUpside && (UnderlyingUpSideSide && GammaUpSide))
                return true;
            else if (TriggerType == TriggerType.BothDownSide && (UnderlyingDownSide && GammaDownSide))
                return true;
            else if (TriggerType == TriggerType.UUGD && (UnderlyingUpSideSide && GammaDownSide))
                return true;
            else if (TriggerType == TriggerType.UDGU && (UnderlyingDownSide && GammaUpSide))
                return true;

            return false;
        }
    }
}
