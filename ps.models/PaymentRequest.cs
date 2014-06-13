using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.models
{
    public class PaymentRequest
    {
        public int ProfileID { get; set; }
        public TransactionTypes TranactionType { get; set; }
        public string ResponseUrl { get; set; }
        public string CancelUrl { get; set; }
        public PaymentDetail PaymentDetail { get; set; }
    }
}
