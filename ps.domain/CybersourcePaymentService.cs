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
    public class CybersourcePaymentService :  CybersourceServiceBase, IPaymentService
    {
        public models.TransactionResponse Sales(models.Profile profile, string paymentToken, models.TransactionRequest transactionRequest)
        {
            var merchantId = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "merchant_id").SettingValue;
            var transactionKey = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transaction_key").SettingValue;
            var serviceEndPoint = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transactionProcessorUrl").SettingValue;

            var request = CreateRequest();
            request.merchantID = merchantId;
            request.merchantReferenceCode = transactionRequest.ReferenceNumber;

            request.ccAuthService = new CCAuthService();
            request.ccAuthService.run = "true";

            request.ccCaptureService = new CCCaptureService();
            request.ccCaptureService.run = "true";
            request.ccCaptureService.purchasingLevel = "3";

            request.recurringSubscriptionInfo = new RecurringSubscriptionInfo();
            request.recurringSubscriptionInfo.subscriptionID = paymentToken;

            request.purchaseTotals = new PurchaseTotals();
            request.purchaseTotals.currency = transactionRequest.Currency;
            request.purchaseTotals.grandTotalAmount = FormatPrice(transactionRequest.Amount);
            request.purchaseTotals.taxAmount = FormatPrice(transactionRequest.TaxAmount);

            //Level II & III info
            request.invoiceHeader = new InvoiceHeader { userPO = transactionRequest.CustomerPO };
            request.purchaseTotals.dutyAmount = "0.00";
            request.purchaseTotals.discountAmount = "0.00";

            if (transactionRequest.ShipTo != null)
            {
                request.shipTo = new ShipTo
                {
                    city = transactionRequest.ShipTo.City,
                    country = transactionRequest.ShipTo.Country,
                    email = transactionRequest.ShipTo.EmailAddress,
                    firstName = transactionRequest.ShipTo.FirstName,
                    lastName = transactionRequest.ShipTo.LastName,
                    postalCode = transactionRequest.ShipTo.PostalCode,
                    state = transactionRequest.ShipTo.State,
                    street1 = transactionRequest.ShipTo.StreetLine1,
                    street2 = transactionRequest.ShipTo.StreetLine2,                    
                };
            }
            if (transactionRequest.ShipFrom != null)
            {
                request.shipFrom = new ShipFrom
                {
                    country = transactionRequest.ShipFrom.Country,
                    email = transactionRequest.ShipFrom.EmailAddress,
                    firstName = transactionRequest.ShipFrom.FirstName,
                    lastName = transactionRequest.ShipFrom.LastName,
                    postalCode = transactionRequest.ShipFrom.PostalCode,
                    state = transactionRequest.ShipFrom.State,
                    street1 = transactionRequest.ShipFrom.StreetLine1,
                    street2 = transactionRequest.ShipFrom.StreetLine2,         
                };
            }
            if (transactionRequest.LineItems != null)
            {
                List<Item> items = new List<Item>();
                foreach (var i in transactionRequest.LineItems)
                {
                    items.Add(new Item
                    {
                        productCode = i.ProductCode,
                        productName = i.ProductName,
                        productSKU = i.ProductSKU,
                        productDescription = i.ProductDescription,
                        unitPrice = FormatPrice(i.UnitPrice),
                        quantity = i.Quantity.ToString(),
                        taxRate = i.TaxRate.ToString(),
                        taxAmount = FormatPrice(i.TaxAmount),
                        discountAmount = FormatPrice(i.DiscountAmount),
                        totalAmount = FormatPrice(i.UnitPrice),
                        commodityCode = i.CommodityCode,
                        unitOfMeasure = "EA",
                    });
                }

                request.item = items.ToArray();
            }
            var client = GetCybersourceService(serviceEndPoint, merchantId, transactionKey);

            var reply = client.runTransaction(request);

            return ProcessReply(profile, reply);
        }


        public models.TransactionResponse Refund(models.Profile profile, string originalTranactionId, models.TransactionRequest transactionRequest)
        {
            var merchantId = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "merchant_id").SettingValue;
            var transactionKey = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transaction_key").SettingValue;
            var serviceEndPoint = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transactionProcessorUrl").SettingValue;

            var request = CreateRequest();
            request.merchantID = merchantId;
            request.merchantReferenceCode = transactionRequest.ReferenceNumber;

            request.ccCreditService = new CCCreditService();
            request.ccCreditService.run = "true";
            request.ccCreditService.captureRequestID = originalTranactionId;
            request.ccCreditService.purchasingLevel = "3";

            PurchaseTotals purchaseTotals = new PurchaseTotals();
            purchaseTotals.currency = transactionRequest.Currency;
            purchaseTotals.grandTotalAmount = FormatPrice(transactionRequest.Amount);
            request.purchaseTotals = purchaseTotals;

            request.invoiceHeader = new InvoiceHeader { userPO = transactionRequest.CustomerPO };
            request.purchaseTotals.dutyAmount = "0.00";
            request.purchaseTotals.discountAmount = "0.00";

            request.invoiceHeader = new InvoiceHeader { userPO = transactionRequest.CustomerPO };
            request.purchaseTotals.dutyAmount = "0.00";
            request.purchaseTotals.discountAmount = "0.00";

            if (transactionRequest.ShipTo != null)
            {
                request.shipTo = new ShipTo
                {
                    city = transactionRequest.ShipTo.City,
                    country = transactionRequest.ShipTo.Country,
                    email = transactionRequest.ShipTo.EmailAddress,
                    firstName = transactionRequest.ShipTo.FirstName,
                    lastName = transactionRequest.ShipTo.LastName,
                    postalCode = transactionRequest.ShipTo.PostalCode,
                    state = transactionRequest.ShipTo.State,
                    street1 = transactionRequest.ShipTo.StreetLine1,
                    street2 = transactionRequest.ShipTo.StreetLine2,                    
                };
            }
            if (transactionRequest.ShipFrom != null)
            {
                request.shipFrom = new ShipFrom
                {
                    country = transactionRequest.ShipFrom.Country,
                    email = transactionRequest.ShipFrom.EmailAddress,
                    firstName = transactionRequest.ShipFrom.FirstName,
                    lastName = transactionRequest.ShipFrom.LastName,
                    postalCode = transactionRequest.ShipFrom.PostalCode,
                    state = transactionRequest.ShipFrom.State,
                    street1 = transactionRequest.ShipFrom.StreetLine1,
                    street2 = transactionRequest.ShipFrom.StreetLine2,         
                };
            }
            if (transactionRequest.LineItems != null)
            {
                List<Item> items = new List<Item>();
                foreach (var i in transactionRequest.LineItems)
                {
                    items.Add(new Item
                    {
                        productCode = i.ProductCode,
                        productName = i.ProductName,
                        productSKU = i.ProductSKU,
                        productDescription = i.ProductDescription,
                        unitPrice = FormatPrice(i.UnitPrice),
                        quantity = i.Quantity.ToString(),
                        taxRate = i.TaxRate.ToString(),
                        taxAmount = FormatPrice(i.TaxAmount),
                        discountAmount = FormatPrice(i.DiscountAmount),
                        totalAmount = FormatPrice(i.UnitPrice),
                        commodityCode = i.CommodityCode,
                        unitOfMeasure = "EA",
                    });
                }

                request.item = items.ToArray();
            }
            var client = GetCybersourceService(serviceEndPoint, merchantId, transactionKey);

            var reply = client.runTransaction(request);

            return ProcessReply(profile, reply);
        }

        public models.TransactionResponse Void(models.Profile profile, string originalTranactionId, string referenceNumber)
        {
            var merchantId = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "merchant_id").SettingValue;
            var transactionKey = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transaction_key").SettingValue;
            var serviceEndPoint = profile.Provider.ProviderSettings.FirstOrDefault(p => p.SettingName == "transactionProcessorUrl").SettingValue;

            var request = CreateRequest();
            request.merchantID = merchantId;
            request.merchantReferenceCode = referenceNumber;

            request.voidService = new VoidService();
            request.voidService.run = "true";
            request.voidService.voidRequestID = originalTranactionId;
            
            var client = GetCybersourceService(serviceEndPoint, merchantId, transactionKey);

            var reply = client.runTransaction(request);

            return ProcessReply(profile, reply);
        }

        protected models.TransactionResponse ProcessReply(Profile profile, ReplyMessage reply)
        {
            var response = new models.TransactionResponse();
            response.Status = true;
            response.ReasonCode = reply.reasonCode;
            if (reply.reasonCode != "100")
            {
                response.Status = false;
            }
            else
            {
                response.TransactionId = string.Format("p{0}-{1}", profile.ProfileID, reply.requestID);
            }
            return response;
        }
        
    }
}
