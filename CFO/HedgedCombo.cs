using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO
{
    public class HedgedCombo
    {
        Dictionary<string, Combo> dicCombo = new Dictionary<string, Combo>();

        public double GetFPL()
        {
            return dicCombo.Values.Sum((combo) => combo.CalculateFPL());
        }

        protected bool Vaildate()
        {
            foreach(string key in dicCombo.Keys)
            {
                if (dicCombo[key].Symbol != key)
                    return false;
            }

            return true;
        }
    }
}
