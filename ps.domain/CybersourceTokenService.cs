using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ps.domain.Cybersource;
using ps.models;

namespace ps.domain
{
    public class CybersourceTokenService : CybersourceServiceBase, ITokenService
    {
        public UpdateTokenResponse Update(Profile profile, string token, CreditCard creditCard)
        {
            var merchantId = GetMerchantId(profile);
            var transactionKey = GetTransactionKey(profile);
            var serviceEndPoint = GetServiceEndPoint(profile);
            var request = CreateRequest();

            request.merchantID = merchantId;
            request.merchantReferenceCode = DateTime.Now.Ticks.ToString();

            request.paySubscriptionUpdateService = new PaySubscriptionUpdateService();
            request.paySubscriptionUpdateService.run = "true";
            
            request.card = new Card();
            request.card.expirationMonth = creditCard.ExpirationMonth;
            request.card.expirationYear = creditCard.ExpirationYear;

            request.recurringSubscriptionInfo = new RecurringSubscriptionInfo();
            request.recurringSubscriptionInfo.subscriptionID = token;

            var client = GetCybersourceService(serviceEndPoint, merchantId, transactionKey);

            var reply = client.runTransaction(request);
            var response = new UpdateTokenResponse();
            response.Status = true;
            response.ReasonCode = reply.reasonCode;
            if (reply.reasonCode != "100")
            {
                response.Status = false;
                response.Message = reply.reasonCode + ". " + string.Join(",", reply.missingField);
            }
            return response;

        }

        public DeleteTokenResponse Delete(Profile profile, string token)
        {
            var merchantId = GetMerchantId(profile);
            var transactionKey = GetTransactionKey(profile);
            var serviceEndPoint = GetServiceEndPoint(profile);
            var request = CreateRequest();
            request.merchantID = merchantId;
            request.merchantReferenceCode = DateTime.Now.Ticks.ToString();

            request.paySubscriptionDeleteService = new PaySubscriptionDeleteService();
            request.paySubscriptionDeleteService.run = "true";

            request.recurringSubscriptionInfo = new RecurringSubscriptionInfo();
            request.recurringSubscriptionInfo.subscriptionID = token;

            var client = GetCybersourceService(serviceEndPoint, merchantId, transactionKey);

            var reply = client.runTransaction(request);
            var response = new DeleteTokenResponse();
            response.Status = true;
            response.ReasonCode = reply.reasonCode;
            if (reply.reasonCode != "100")
            {
                response.Status = false;
                response.Message = reply.reasonCode + ". " + string.Join(",", reply.missingField);
            }
            return response;
        }

        public GetTokenDetailResponse Get(Profile profile, string token)
        {
            var merchantId = GetMerchantId(profile);
            var transactionKey = GetTransactionKey(profile);
            var serviceEndPoint = GetServiceEndPoint(profile);

            var request = CreateRequest();
            request.merchantID = merchantId;
            request.merchantReferenceCode = DateTime.Now.Ticks.ToString();
            request.paySubscriptionRetrieveService = new PaySubscriptionRetrieveService();
            request.paySubscriptionRetrieveService.run = "true";

            request.recurringSubscriptionInfo = new RecurringSubscriptionInfo();
            request.recurringSubscriptionInfo.subscriptionID = token;

            var client = GetCybersourceService(serviceEndPoint, merchantId, transactionKey);

            var reply = client.runTransaction(request);
            var response = new GetTokenDetailResponse();
            response.Status = true;
            response.ReasonCode = reply.reasonCode;
            if (reply.reasonCode != "100")
            {
                response.Status = false;
                response.Message = reply.reasonCode + ". " + string.Join(",", reply.missingField); 
            }
            else
            {
                response.CreditCard = new CreditCard
                {
                    CardNumber = reply.paySubscriptionRetrieveReply.cardAccountNumber,
                    CardType = reply.paySubscriptionRetrieveReply.cardType,
                    ExpirationMonth = reply.paySubscriptionRetrieveReply.cardExpirationMonth,
                    ExpirationYear = reply.paySubscriptionRetrieveReply.cardExpirationYear,                    
                };
                response.BillingContact = new Contact
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

        public CreateTokenResponse Create(Profile profile, CreditCard creditCard, Contact billingContact)
        {
            var merchantId = GetMerchantId(profile);
            var transactionKey = GetTransactionKey(profile);
            var serviceEndPoint = GetServiceEndPoint(profile);

            var request = CreateRequest();
            request.merchantID = merchantId;
            request.merchantReferenceCode = DateTime.Now.Ticks.ToString();
            request.paySubscriptionCreateService = new PaySubscriptionCreateService();
            request.paySubscriptionCreateService.run = "true";

            request.recurringSubscriptionInfo = new RecurringSubscriptionInfo() { frequency = "on-demand"};
            request.card = new Card()
            {
                cardType = creditCard.CardType,
                accountNumber = creditCard.CardNumber,
                expirationMonth = creditCard.ExpirationMonth,
                expirationYear = creditCard.ExpirationYear
            };
            request.billTo = new BillTo()
            {
                firstName = billingContact.FirstName,
                lastName = billingContact.LastName,
                city = billingContact.City,
                country = billingContact.Country,
                email = billingContact.EmailAddress,
                postalCode = billingContact.PostalCode,
                state = billingContact.State,
                street1 = billingContact.StreetLine1,
            };
            request.purchaseTotals = new PurchaseTotals() { currency = "USD" };

            var client = GetCybersourceService(serviceEndPoint, merchantId, transactionKey);

            var reply = client.runTransaction(request);
            var response = new CreateTokenResponse();
            response.Status = true;
            response.ReasonCode = reply.reasonCode;
            if (reply.reasonCode != "100")
            {
                response.Status = false;
                response.Message = reply.reasonCode + ". " + string.Join(",", reply.missingField); 
            }
            else
            {
                response.Token = reply.paySubscriptionCreateReply.subscriptionID;       
            }
            return response;
        }        
    }
}
