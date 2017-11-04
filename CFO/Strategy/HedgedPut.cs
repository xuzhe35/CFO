using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO.Strategy
{
    public class HedgedPut : Combo
    {
        public Position PosPut { get; set; }
        public Position PosSpot { get; set; }

        public Option Put { get; set; }

        /// <summary>
        /// DDH追求的固定Delta值
        /// </summary>
        public double FixedDelta { get; set; }
        /// <summary>
        /// DDH 的间距
        /// </summary>
        public double DDH_Distance { get; set; }

        public HedgedPut()
        {

        }

        public HedgedPut(Position LegPut, Position LegSpot)
        {
            PosPut = LegPut;
            PosSpot = LegSpot;

            Legs.Add(LegPut);
            Legs.Add(LegSpot);

            Put = (Option)PosPut.Asset;

            this.LoseType = LoseType.LimitedLose;
            this.ProfitType = ProfitType.UnLimitedProfit;

            if (VaildateSameSymbol() && VaildateHedgedPut())
                IsVaildated = true;
            else
                IsVaildated = false;
        }

        public bool VaildateHedgedPut()
        {
            IsVaildated = false;

            if (Legs.Count != 2)
                throw new VaildateException("必须是两腿的");

            var OptionLegsCount = Legs.Count((leg) => leg.Asset.GetType() == typeof(Option));
            var SpotLegsCount = Legs.Count((leg) => leg.Asset.GetType() == typeof(STK));

            if (!(OptionLegsCount == 1 && SpotLegsCount == 1))
                throw new VaildateException("HedgePut 必须是做多现货和看跌期权");

            if (PosSpot.Quantity < 0 || PosPut.Quantity < 0)
                throw new VaildateException("现货和期权都必须做多");

            if (PosSpot.Quantity < PosPut.Quantity)
                throw new VaildateException("现货做多的量不能大于期货的保护量");

            return true;
        }

        public override double CalculateMaxLose()
        {
            MaxLose = Legs.Sum((leg) => leg.CalculateFinalProfit(Put.StrikePrice));

            if (MaxLose > 0)
                throw new Exception("Bear Spread 的最大损失不应大于0");

            return MaxLose;
        }

        /// <summary>
        /// 设置DDH的参数
        /// </summary>
        /// <param name="TargetFixedDelta">固定的DDH目标Delta，如果是中性组合，则这个值应该设置为0</param>
        /// <param name="TradeDistance">DDH的阈值距离，DDH的敏感度</param>
        public void DDH_Setting(double TargetFixedDelta,double TradeDistance)
        {
            FixedDelta = TargetFixedDelta;
            DDH_Distance = TradeDistance;
        }

        /// <summary>
        /// 检测是否需要DDH
        /// </summary>
        /// <returns></returns>
        public double CheckDDH()
        {
            double delta = ComboGreeks.Delta;
            double TargetSpot = 0;

            if (delta > FixedDelta + DDH_Distance)
            {
                TargetSpot = delta - FixedDelta;
            }
            else if (delta < FixedDelta - DDH_Distance)
            {
                TargetSpot = FixedDelta - delta;
            }

            return TargetSpot;
        }
    }
}
