using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO.Strategy
{
    public class BullCollar : Combo
    {
        public override void TypeJudge()
        {
            this.LoseType = LoseType.LimitedLose;
            this.ProfitType = ProfitType.LimitedProfit;
        }
    }
}
