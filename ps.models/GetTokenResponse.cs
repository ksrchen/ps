using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.models
{
    public class GetTokenDetailResponse : ResponseBase
    {
        public CreditCard CreditCard { get; set; }
        public Contact BillingContact { get; set; }
    }
}
