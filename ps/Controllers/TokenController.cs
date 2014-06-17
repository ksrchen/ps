using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using ps.domain;
using ps.models;

namespace ps.Controllers
{
    public class TokenController : BaseApiController
    {
        public TokenController(IServiceLocator serviceLocator, IProfileDomain profileDomain) : base(serviceLocator, profileDomain)
        {
        }
        /// <summary>
        /// Retrieve credit card detail and billing contact associated with a payment token
        /// </summary>
        /// <param name="id">payment token</param>
        /// <returns></returns>
        public HttpResponseMessage Get(string id)
        {
            try
            {
                int profileId = 0;
                string providerToken;
                ParseToken(id, out profileId, out providerToken);

                var profile = _profileDomain.Get(profileId);
                var service = _serviceLocator.GetTokenService(profile.Provider.ProviderType);
                var result = service.Get(profile, providerToken);
                return Request.CreateResponse<GetTokenDetailResponse>(HttpStatusCode.OK, result);
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }

        }
        /// <summary>
        /// Update the credit card expiration date associated with a payment token
        /// </summary>
        /// <param name="id">payment token</param>
        /// <param name="request">new credit expiration date</param>
        /// <returns></returns>
        public HttpResponseMessage Put(string id, [FromBody]UpdateTokenRequest request)
        {
            try
            {
                int profileId = 0;
                string providerToken;
                ParseToken(id, out profileId, out providerToken);

                var profile = _profileDomain.Get(profileId);
                var service = _serviceLocator.GetTokenService(profile.Provider.ProviderType);
                var result = service.Update(profile, providerToken, request.CreditCard);
                return Request.CreateResponse<UpdateTokenResponse>(HttpStatusCode.OK, result);
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }

        }
        /// <summary>
        /// Remove a payment token from the payment provider
        /// </summary>
        /// <param name="id">payment token</param>
        /// <returns></returns>
        public HttpResponseMessage Delete(string id)
        {
            try
            {
                int profileId = 0;
                string providerToken;
                ParseToken(id, out profileId, out providerToken);

                var profile = _profileDomain.Get(profileId);
                var service = _serviceLocator.GetTokenService(profile.Provider.ProviderType);
                var result = service.Delete(profile, providerToken);
                return Request.CreateResponse<DeleteTokenResponse>(HttpStatusCode.OK, result);
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }

        }
        
    }
}
