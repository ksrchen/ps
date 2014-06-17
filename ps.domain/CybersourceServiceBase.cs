using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ps.domain.Cybersource;
using ps.models;

namespace ps.domain
{
    public class CybersourceServiceBase
    {
        protected static Cybersource.RequestMessage CreateRequest()
        {

            RequestMessage request = new RequestMessage();

            request.clientLibrary = ".NET WCF";
            request.clientLibraryVersion = Environment.Version.ToString();
            request.clientEnvironment =
                Environment.OSVersion.Platform +
                Environment.OSVersion.Version.ToString();

            return request;
        }


        protected static Cybersource.TransactionProcessorClient GetCybersourceService(string serviceEndpoint,
            string merchantId,
            string transactionKey)
        {
            var binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;

            var address = new EndpointAddress(serviceEndpoint);
            var Service = new Cybersource.TransactionProcessorClient(binding, address);
            Service.ChannelFactory.Credentials.UserName.UserName = merchantId;
            Service.ChannelFactory.Credentials.UserName.Password = transactionKey;
            return Service;

        }

       
        protected string FormatPrice(int price)
        {
            return ((decimal)price / 100.0m).ToString("0.00");
        }
        protected string GetMerchantId(Profile profile)
        {
            return profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "merchant_id").SettingValue;
        }
        protected string GetTransactionKey(Profile profile)
        {
            return profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transaction_key").SettingValue;
        }
        protected string GetServiceEndPoint(Profile profile)
        {
            return profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transactionProcessorUrl").SettingValue;
        }
    }
}
