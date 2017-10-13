using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO
{
    public class Position
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 该仓位持有的金融资产
        /// </summary>
        public CFO Asset { get; set; }

        /// <summary>
        /// 平均建仓价格
        /// </summary>
        public double AvgCost { get; set; }

        /// <summary>
        /// 持仓量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 头寸是否已经了结
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        /// 资产的浮动盈亏
        /// </summary>
        public double FloatingPL { get; set; }

        /// <summary>
        /// 计算资产价格的浮盈浮亏
        /// </summary>
        /// <returns></returns>
        public bool CalculateFPL()
        {
            if (Asset.CurrentPrice != null)
            {
                if (Quantity >= 0)
                    FloatingPL = (Asset.CurrentPrice.Bid - AvgCost) * Quantity;
                else
                    FloatingPL = (AvgCost - Asset.CurrentPrice.Ask) * Math.Abs(Quantity);

                return true;
            }
            else
                return false;
        }
    }
}
