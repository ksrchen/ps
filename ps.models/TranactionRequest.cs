using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.models
{
    public class TransactionRequest
    {
        public TransactionTypes TransactionType { get; set; }
        public string ReferenceNumber { get; set; }
        public string PaymentToken { get; set; }
        public string TransactionId { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public int TaxAmount { get; set; }
        public string CustomerPO { get; set; }
        public Contact ShipTo { get; set; }
        public Contact ShipFrom { get; set; }
        public List<LineItem> LineItems { get; set; }

    }
}
