using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Description;
using ps.domain;
using ps.models;

namespace ps.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class TransactionController : BaseApiController
    {
        public TransactionController(IServiceLocator serviceLocator, IProfileDomain profileDomain) :
            base(serviceLocator, profileDomain)
        {
        }
        /// <summary>
        /// Process a payment transaction request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromBody] TransactionRequest request)
        {
            try
            {
                switch (request.TransactionType)
                {
                    case TransactionTypes.Refund:
                        {
                            int profileId = 0;
                            string transactionId;
                            ParseTransactionId(request.TransactionId, out profileId, out transactionId);

                            var profile = _profileDomain.Get(profileId);
                            var service = _serviceLocator.GetPaymentService(profile.Provider.ProviderType);
                            var resp = service.Refund(profile, transactionId, request);

                            return Request.CreateResponse<TransactionResponse>(HttpStatusCode.OK, resp);
                        }
                    case TransactionTypes.Sales:
                        {
                            int profileId = 0;
                            string providerToken;
                            ParseToken(request.PaymentToken, out profileId, out providerToken);

                            var profile = _profileDomain.Get(profileId);
                            var service = _serviceLocator.GetPaymentService(profile.Provider.ProviderType);
                            var resp = service.Sales(profile, providerToken, request);

                            return Request.CreateResponse<TransactionResponse>(HttpStatusCode.OK, resp);
                        }
                    case TransactionTypes.Void:
                        {
                            int profileId = 0;
                            string transactionId;
                            ParseTransactionId(request.TransactionId, out profileId, out transactionId);

                            var profile = _profileDomain.Get(profileId);
                            var service = _serviceLocator.GetPaymentService(profile.Provider.ProviderType);
                            var resp = service.Void(profile, transactionId, request.ReferenceNumber);

                            return Request.CreateResponse<TransactionResponse>(HttpStatusCode.OK, resp);
                        }
                }
                throw new Exception(string.Format("unexpected transaction type {0}", request.TransactionType));
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
    }
}
