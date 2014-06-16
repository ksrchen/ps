using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ps.models;

namespace ps.domain
{
    public interface IPaymentService
    {
        TransactionResponse Sales(Profile profile, string paymentToken, TranactionRequest request);
        TransactionResponse Refund(Profile profile, string originalTranactionId, TranactionRequest request);
        TransactionResponse Void(Profile profile, string originalTranactionId, string referenceNumber);
    }
}
