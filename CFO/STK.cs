using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO
{
    public class STK : CFO
    {
        public STK(string strSymbol)
        {
            Symbol = strSymbol;
        }

        /// <summary>
        /// 做空需要付出的股息率
        /// </summary>
        public double dividend_rate { get; set; }
    }
}
