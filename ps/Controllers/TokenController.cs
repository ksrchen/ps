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
    public class TokenController : ApiController
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly IProfileDomain _profileDomain;
        public TokenController(IServiceLocator serviceLocator, IProfileDomain profileDomain)
        {
            _serviceLocator = serviceLocator;
            _profileDomain = profileDomain;
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
        private static void ParseToken(string inputToken, out int profileId, out string providerToken)
        {
            const string pattern = "p([0-9]*)-([a-zA-Z0-9]*)";
            var matches = Regex.Match(inputToken, pattern);
            if (matches.Success)
            {
                profileId = int.Parse(matches.Groups[1].ToString());
                providerToken = matches.Groups[2].ToString();
            }
            else
            {
                throw new Exception(string.Format("invalid token format"));
            }
        }
    }
}
