using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using ps.domain;

namespace ps.Controllers
{
   
    public abstract class BaseApiController : ApiController
    {
        protected readonly IServiceLocator _serviceLocator;
        protected readonly IProfileDomain _profileDomain;
        public BaseApiController(IServiceLocator serviceLocator, IProfileDomain profileDomain)
        {
            _serviceLocator = serviceLocator;
            _profileDomain = profileDomain;
        }
        protected static void ParseToken(string inputToken, out int profileId, out string providerToken)
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
        protected static void ParseTransactionId(string inputToken, out int profileId, out string providerTransactionId)
        {
            const string pattern = "p([0-9]*)-([a-zA-Z0-9]*)";
            var matches = Regex.Match(inputToken, pattern);
            if (matches.Success)
            {
                profileId = int.Parse(matches.Groups[1].ToString());
                providerTransactionId = matches.Groups[2].ToString();
            }
            else
            {
                throw new Exception(string.Format("invalid transaction id format"));
            }
        }
    }
}
