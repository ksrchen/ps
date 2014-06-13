using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.models
{
    public class PaymentDetail
    {
        public string PaymentTokenID { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public string Currency { get; set; }
        public Contact BillingAddress { get; set; }
    }
}
