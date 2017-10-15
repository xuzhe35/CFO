using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFO.Strategy
{
    public class VaildateException : ApplicationException
    {
        public VaildateException() { }
        public VaildateException(string message)  
            : base(message) { }
        public VaildateException(string message, Exception inner)  
            : base(message, inner) { }
    }
}
