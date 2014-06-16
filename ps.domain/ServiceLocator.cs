using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.domain
{
    public class ServiceLocator : IServiceLocator
    {
        IPaymentService IServiceLocator.GetPaymentService(string providerType)
        {
            return new CybersourcePaymentService();
        }

        ITokenService IServiceLocator.GetTokenService(string providerType)
        {
            return new CybersourceTokenService();
        }
    }
}
