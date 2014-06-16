using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.domain
{
    public class CybersourceTokenService : ITokenService
    {
        public models.UpdateTokenResponse Update(models.Profile profile, string token, models.CreditCard creditCard)
        {
            throw new NotImplementedException();
        }

        public models.DeleteTokenResponse Delete(models.Profile profile, string token)
        {
            throw new NotImplementedException();
        }

        public models.GetTokenResponse Get(models.Profile profile, string token)
        {
            throw new NotImplementedException();
        }
    }
}
