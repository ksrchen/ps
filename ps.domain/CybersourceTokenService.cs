using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ps.domain.Cybersource;

namespace ps.domain
{
    public class CybersourceTokenService : CybersourceServiceBase, ITokenService
    {
        public models.UpdateTokenResponse Update(models.Profile profile, string token, models.CreditCard creditCard)
        {
            var merchantId = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "merchant_id").SettingValue;
            var transactionKey = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transaction_key").SettingValue;
            var serviceEndPoint = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transactionProcessorUrl").SettingValue;
            var request = CreateRequest();

            request.paySubscriptionUpdateService = new PaySubscriptionUpdateService();
            request.paySubscriptionUpdateService.run = "true";
            
            request.card = new Card();
            request.card.expirationMonth = creditCard.ExpirationMonth;
            request.card.expirationYear = creditCard.ExpirationYear;

            request.recurringSubscriptionInfo = new RecurringSubscriptionInfo();
            request.recurringSubscriptionInfo.subscriptionID = token;

            var client = GetCybersourceService(serviceEndPoint, merchantId, transactionKey);

            var reply = client.runTransaction(request);
            var response = new models.UpdateTokenResponse();
            response.Status = true;
            response.ReasonCode = reply.reasonCode;
            if (reply.reasonCode != "100")
            {
                response.Status = false;
                response.Message = reply.reasonCode + ". " + reply.missingField;
            }
            return response;

        }

        public models.DeleteTokenResponse Delete(models.Profile profile, string token)
        {
            var merchantId = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "merchant_id").SettingValue;
            var transactionKey = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transaction_key").SettingValue;
            var serviceEndPoint = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transactionProcessorUrl").SettingValue;
            var request = CreateRequest();

            request.paySubscriptionDeleteService = new PaySubscriptionDeleteService();
            request.paySubscriptionRetrieveService.run = "true";

            request.recurringSubscriptionInfo = new RecurringSubscriptionInfo();
            request.recurringSubscriptionInfo.subscriptionID = token;

            var client = GetCybersourceService(serviceEndPoint, merchantId, transactionKey);

            var reply = client.runTransaction(request);
            var response = new models.DeleteTokenResponse();
            response.Status = true;
            response.ReasonCode = reply.reasonCode;
            if (reply.reasonCode != "100")
            {
                response.Status = false;
                response.Message = reply.reasonCode + ". " + reply.missingField;
            }
            return response;
        }

        public models.GetTokenResponse Get(models.Profile profile, string token)
        {
            var merchantId = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "merchant_id").SettingValue;
            var transactionKey = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transaction_key").SettingValue;
            var serviceEndPoint = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transactionProcessorUrl").SettingValue;

            var request = CreateRequest();

            request.paySubscriptionRetrieveService = new PaySubscriptionRetrieveService();
            request.paySubscriptionRetrieveService.run = "true";

            request.recurringSubscriptionInfo = new RecurringSubscriptionInfo();
            request.recurringSubscriptionInfo.subscriptionID = token;

            var client = GetCybersourceService(serviceEndPoint, merchantId, transactionKey);

            var reply = client.runTransaction(request);
            var response = new models.GetTokenResponse();
            response.Status = true;
            response.ReasonCode = reply.reasonCode;
            if (reply.reasonCode != "100")
            {
                response.Status = false;
                response.Message = reply.reasonCode + ". " + reply.missingField;
            }
            else
            {
                response.CreditCard = new models.CreditCard
                {
                    CardNumber = reply.paySubscriptionRetrieveReply.cardAccountNumber,
                    CardType = reply.paySubscriptionRetrieveReply.cardType,
                    ExpirationMonth = reply.paySubscriptionRetrieveReply.cardExpirationMonth,
                    ExpirationYear = reply.paySubscriptionRetrieveReply.cardExpirationYear,                    
                };
                response.BillingContact = new models.Contact
                {
                    FirstName = reply.paySubscriptionRetrieveReply.firstName,
                    LastName = reply.paySubscriptionRetrieveReply.lastName,
                    City = reply.paySubscriptionRetrieveReply.city,
                    Country = reply.paySubscriptionRetrieveReply.country,
                    CompanyName = reply.paySubscriptionRetrieveReply.companyName,
                    EmailAddress = reply.paySubscriptionRetrieveReply.email,
                    PostalCode = reply.paySubscriptionRetrieveReply.postalCode,
                    State = reply.paySubscriptionRetrieveReply.state,
                    StreetLine1 = reply.paySubscriptionRetrieveReply.street1,
                    StreetLine2 = reply.paySubscriptionRetrieveReply.street2,                   
                };
            }
            return response;
        }

    }
}
