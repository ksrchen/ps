using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.models
{
    public class ResponseBase
    {
        public bool Status { get; set; }
        public string ReasonCode { get; set; }
        public string Message { get; set; }
    }
}
