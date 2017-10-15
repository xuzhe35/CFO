using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO
{
    /// <summary>
    /// 期权行权类型
    /// </summary>
    public enum OptionRight
    {
        CALL,
        PUT
    }

    /// <summary>
    /// 期权内在价值类型
    /// </summary>
    public enum OptionType
    {
        ATM,
        ITM,
        OTM
    }

    public enum ComboStrategy
    {
        BearSpread,
        Collar,
        CBS
    }

    public enum ProfitType
    {
        UnLimitedProfit,
        LimitedProfit
    }

    public enum LoseType
    {
        UnlimitedLose,
        LimitedLose
    }

    public enum TriggerType
    {
        UnderlyingUpside,
        UnderlyingDownsid,
        GammaUpside,
        GammaDownSide,
        BothUpside,
        BothDownSide,
        UUGD,           //Underlying Upside Gamma DownSide
        UDGU            //Underlying Downside Gamma Upside
    }
}
