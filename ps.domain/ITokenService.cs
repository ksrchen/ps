using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ps.models;

namespace ps.domain
{
    public interface ITokenService
    {
        UpdateTokenResponse Update(Profile profile, string token, CreditCard creditCard);
        DeleteTokenResponse Delete(Profile profile, string token);
        GetTokenDetailResponse Get(Profile profile, string token);
        CreateTokenResponse Create(Profile profile, CreditCard creditCard, Contact billingContact);
    }
}
