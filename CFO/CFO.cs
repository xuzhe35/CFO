using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO
{
    /// <summary>
    /// Common financial object 通用金融资产的父类
    /// </summary>
    public class CFO
    {
        /// <summary>
        /// 唯一标识符。例如 STK.SPY 或者 OPT.SPY.C.255.20171117
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 该资产的Symbol，与资产类型无关
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 当前市场价格
        /// </summary>
        public Price CurrentPrice { get; set; }
    }
}
