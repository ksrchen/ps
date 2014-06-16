using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.domain
{
    public interface IServiceLocator
    {
        IPaymentService GetPaymentService(string providerType);
        ITokenService GetTokenService(string providerType);
    }
}
