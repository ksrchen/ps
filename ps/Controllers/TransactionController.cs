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
    public class TransactionController : ApiController
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly IProfileDomain _profileDomain;
        private const string pattern =  "p([0-9]*)-([a-zA-Z0-9]*)";
        public TransactionController(IServiceLocator serviceLocator, IProfileDomain profileDomain)
        {
            _serviceLocator = serviceLocator;
            _profileDomain = profileDomain;
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
                            var matches = Regex.Match(request.TransactionId, pattern);
                            int profileId = int.Parse(matches.Groups[1].ToString());
                            string transactionId = matches.Groups[2].ToString();

                            var profile = _profileDomain.Get(profileId);
                            var service = _serviceLocator.GetPaymentService(profile.Provider.ProviderType);
                            var resp = service.Refund(profile, transactionId, request);

                            return Request.CreateResponse<TransactionResponse>(HttpStatusCode.OK, resp);
                        }
                    case TransactionTypes.Sales:
                        {
                            var matches = Regex.Match(request.PaymentToken, pattern);
                            int profileId = int.Parse(matches.Groups[1].ToString());
                            string paymentToken = matches.Groups[2].ToString();

                            var profile = _profileDomain.Get(profileId);
                            var service = _serviceLocator.GetPaymentService(profile.Provider.ProviderType);
                            var resp = service.Sales(profile, paymentToken, request);

                            return Request.CreateResponse<TransactionResponse>(HttpStatusCode.OK, resp);
                        }
                    case TransactionTypes.Void:
                        {
                            var matches = Regex.Match(request.TransactionId, pattern);
                            int profileId = int.Parse(matches.Groups[1].ToString());
                            string transactionId = matches.Groups[2].ToString();

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
