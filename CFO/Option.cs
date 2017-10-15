using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO
{
    public class Option : CFO
    {
        /// <summary>
        /// 执行价
        /// </summary>
        public double StrikePrice { get; set; }

        /// <summary>
        /// 期权的类型，看涨或者看跌
        /// </summary>
        public OptionRight Right { get; set; }

        /// <summary>
        /// DateTime 类形式的到期日
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// 字符串形式的到期日
        /// </summary>
        public string strExpiryDate { get; set; }

        /// <summary>
        /// 期权的希腊字母
        /// </summary>
        public Greeks Greeks { get; set; }

        /// <summary>
        /// 隐含波动率
        /// </summary>
        public double IV { get; set; }

        public double AskIV { get; set; }
        public double BidIV { get; set; }

        /// <summary>
        /// 期权的内在价值
        /// </summary>
        public double IntrinsicValue { get; set; }

        /// <summary>
        /// 期权的类型，实值虚值还是平值
        /// </summary>
        public OptionType Type { get; set; }

        public Option(string strSymbol,OptionRight orRight,double dbStrikePrice,string ExpiryDateString)
        {
            Symbol = strSymbol;
            strExpiryDate = ExpiryDateString;
            GenExpiryDate();

            Right = orRight;
            StrikePrice = dbStrikePrice;
        }

        public Option(string FomartString)
        {

        }

        /// <summary>
        /// ExpiyDate 转换为字符串
        /// </summary>
        /// <returns></returns>
        public string GetStrExpiryDate ()
        {
            strExpiryDate = ExpiryDate.ToString("yyyyMMdd");
            return strExpiryDate;
        }

        /// <summary>
        /// 生成时间类型的到期日
        /// </summary>
        /// <returns>如果没有字符串形式的则转换失败，返回false；成功返回true</returns>
        public bool GenExpiryDate()
        {
            if (!String.IsNullOrEmpty(strExpiryDate))
            {
                string Year = strExpiryDate.Substring(0, 4);
                string Month = strExpiryDate.Substring(4, 2);
                string Day = strExpiryDate.Substring(6, 2);

                DateTime datetime = new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day));
                ExpiryDate = datetime;

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 输入字符串形式的到期日，自动设置
        /// </summary>
        /// <param name="ExpiryDateString">字符串形式的到期日，例如 20171117</param>
        public void SetExpiryDate(string ExpiryDateString)
        {
            strExpiryDate = ExpiryDateString;
            GenExpiryDate();
        }

        /// <summary>
        /// 判断是平值虚值还是实值
        /// </summary>
        /// <param name="UnderlyingPrice"></param>
        /// <returns></returns>
        public OptionType GetType(Price UnderlyingPrice)
        {
            OptionType result;

            if (UnderlyingPrice.Last == StrikePrice)
                result = OptionType.ATM;
            else
            {
                if ((Right == OptionRight.CALL && UnderlyingPrice.Bid < StrikePrice) ||
                    (Right == OptionRight.PUT && UnderlyingPrice.Ask > StrikePrice))
                {
                    result = OptionType.OTM;
                }
                else
                {
                    result = OptionType.ITM;
                }
            }

            return result;
        }

        /// <summary>
        /// 计算行权后的价值
        /// </summary>
        /// <param name="UnderlyingPrice">标的资产的价格</param>
        /// <returns></returns>
        public double GetIntrinsicValue(Price UnderlyingPrice)
        {
            //获取期权到期时的类型
            OptionType FinalType = GetType(UnderlyingPrice);
            double dbIntrinsicValue = 0;

            if (FinalType == OptionType.ATM || FinalType == OptionType.OTM)
            {
                //如果是虚值期权或者平价，则期权内在价值为0
                dbIntrinsicValue = 0;
            }
            else
            {
                //处理实值期权内在价值的计算问题
                if (Right == OptionRight.CALL)
                {
                    //看涨期权的内在价值计算
                    dbIntrinsicValue = UnderlyingPrice.Bid - StrikePrice;
                }
                else if (Right == OptionRight.PUT)
                {
                    //看跌期权的内在价值计算
                    dbIntrinsicValue = StrikePrice - UnderlyingPrice.Ask;
                }
            }

            return dbIntrinsicValue;
        }
    }
}
