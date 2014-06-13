using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.domain
{
    public class CybersourcePaymentService : IPaymentService
    {
        public models.TransactionResponse Sales(models.Profile profile, models.TranactionRequest request)
        {
            throw new NotImplementedException();
        }

        public models.TransactionResponse Refund(models.Profile profile, models.TranactionRequest request)
        {
            throw new NotImplementedException();
        }

        public models.TransactionResponse Void(models.Profile profile, models.TranactionRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
