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
        public ComboStrategy Type { get; set; }
        public double FloatingPL { get; set; }
        public Greeks ComboGreeks { get; set; }

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
