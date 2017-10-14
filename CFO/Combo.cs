using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO
{
    public class Combo
    {
        public List<Position> MemberPos = new List<Position>();

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
        public bool Vaildate()
        {
            if (MemberPos.Count > 0)
            {
                var symbol = MemberPos[0].Asset.Symbol;
                //如果成员里但凡有一个不是相同的symbol，则返回验证失败
                if (MemberPos.Exists((mp) => mp.Asset.Symbol != symbol))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        private void TypeJudge()
        {
            if(Strategy == ComboStrategy.BearSpread || Strategy == ComboStrategy.Collar)
            {
                ProfitType = ProfitType.LimitedProfit;
                LoseType = LoseType.LimitedLose;
            }
            else if(Strategy == ComboStrategy.CBS)
            {
                ProfitType = ProfitType.UnLimitedProfit;
                LoseType = LoseType.LimitedLose;
            }
        }

        public bool CalculateMaxProfit()
        {
            TypeJudge();

            if (ProfitType == ProfitType.LimitedProfit)
            {
                double final_profit = 0;
                if (Strategy == ComboStrategy.BearSpread || Strategy == ComboStrategy.Collar)
                {
                    MemberPos.ForEach((pos) =>
                    {
                        var asset = pos.Asset;

                        //如果是期权头寸，则最大利润是underlying价格为0的时候
                        if(asset.GetType()==typeof(Option))
                        {
                            var option = (Option)asset;
                            double option_value = option.GetIntrinsicValue(new Price { Ask = 0, Bid = 0, Last = 0 });

                            if(pos.Quantity>0)
                            {
                                final_profit += (option_value - pos.AvgCost) * pos.Quantity;
                            }
                            else
                            {
                                final_profit += (pos.AvgCost - option_value) * Math.Abs(pos.Quantity);
                            }
                        }
                        else if(asset.GetType()==typeof(STK))
                        {
                            //如果是Collar的话，现货为0时候现货头寸的收益就是价格乘以数量
                            final_profit = asset.CurrentPrice.Bid * pos.Quantity;
                        }
                    });

                    MaxProfit = final_profit;

                    return true;
                }

                return true;
            }
            else
                return false;
        }

        public double CalculateFPL()
        {
            MemberPos.ForEach((p) => p.CalculateFPL());
            FloatingPL = MemberPos.Sum((p) => p.FloatingPL);

            return FloatingPL;
        }

        /// <summary>
        /// 计算合并的希腊值，只有同名的资产该方法才有效
        /// </summary>
        public void CalculateGreeks()
        {
            double SumDelta = 0;
            double SumGamma = 0;
            double SumVega = 0;
            double SumTheta = 0;

            foreach(Position pos in MemberPos)
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
    }
}
