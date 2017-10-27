using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO
{
    public class STK : CFO
    {
        public STK(string strFormatString)
        {
            if (strFormatString.StartsWith("STK"))
            {
                Symbol = strFormatString.Substring(4, strFormatString.Length - 4);
                ID = strFormatString;
            }
            else
            {
                Symbol = strFormatString;
                ID = "STK." + strFormatString;
            }
        }

        /// <summary>
        /// 做空需要付出的股息率
        /// </summary>
        public double dividend_rate { get; set; }
    }
}
