using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ps.domain;
using ps.models;

namespace migration
{
    class Program
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger("credit card migration tool");

        private static StreamWriter _file;
        static void Main(string[] args)
        {
            try
            {

                _file = new StreamWriter(@".\result.csv");
                _file.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                    "customerId",
                    "creditCardId",
                    "creditCardTypeId",
                    "expiration",
                    "status",
                    "message",
                    "reasonCode",
                    "token");

                AppDomain.CurrentDomain.SetData("DataDirectory", ".\\");
                var profileDomain = new ProfileDomain();
                var profile = profileDomain.Get(1);

                using (var db = new LARSEntities())
                {
                    int sucessCount = 0;
                    DateTime start = DateTime.Now;

                    _logger.Info("Retrieving credit cards...");

                    var query = db.CCInfo_t.Include("Contact_t").Where(p => p.Active);                    
                    long totalCount = query.LongCount();

                    _logger.InfoFormat("Found {0} credit card(s) ", totalCount);

                    int i = 0;
                    foreach (var item in query)
                    {
                        _logger.InfoFormat("processing {0} of {1}", i++, totalCount);

                        string cardNumber = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.CCNum))
                        {
                            Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "card number is missing");
                            continue;
                        }
                        cardNumber = LNEncryption.LNDecrypt(item.CCNum);

                        string expiryMonth = string.Empty;
                        string expiryYear = string.Empty;

                        if (string.IsNullOrWhiteSpace(item.CCExpDate))
                        {
                            Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "expiration date missing");
                            continue;
                        }

                        var parts = item.CCExpDate.Split(new char[] { '/' });
                        if (parts.Count() >= 2)
                        {
                            expiryMonth = parts[0];
                            expiryYear = parts[1];
                        }
                        else
                        {
                            if (item.CCExpDate.Length == 4)
                            {
                                expiryMonth = item.CCExpDate.Substring(0, 2);
                                expiryYear = item.CCExpDate.Substring(2, 2);
                            }
                            else
                            {
                                Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "invalid expiration date");
                                continue;
                            }
                        }
                        int month = 0;
                        int year = 0;
                        if (!int.TryParse(expiryMonth, out month))
                        {
                            Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "invalid month");
                            continue;
                        }

                        if (!int.TryParse(expiryYear, out year))
                        {
                            Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "invalid year"); 
                            continue;
                        }
                        year = year + 2000;

                        if (new DateTime(year, month, 1) < DateTime.Now.Date)
                        {
                            Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "credit card expired");
                            continue;
                        }
                        string creditCardType = string.Empty;
                        switch ((int)item.CCTypeID.GetValueOrDefault())
                        {
                            case 1719: //master
                                creditCardType = "002";
                                break;
                            case 1720: //AMEX
                                creditCardType = "003";
                                break;
                            case 1722: //DISC
                                creditCardType = "004";
                                break;
                            case 1723: //VISA
                                creditCardType = "001";
                                break;
                            default:
                                Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "invalid credit card type");
                                continue;
                        }
                        string email = item.Contact_t.EmailAddress;
                        if (string.IsNullOrWhiteSpace(item.Contact_t.EmailAddress))
                        {
                            //Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "email address is missing");
                            //continue;
                            email = "placeholder@loopnet.com";
                        }
                        StringBuilder sb = new StringBuilder();
                        if (string.IsNullOrWhiteSpace(item.Contact_t.FirstName))
                        {
                            sb.Append("[FirstName] ");
                        }
                        if (string.IsNullOrWhiteSpace(item.Contact_t.LastName))
                        {
                            sb.Append("[LastName] ");
                        }
                        if (string.IsNullOrWhiteSpace(item.Contact_t.City))
                        {
                            sb.Append("[City] ");
                        }
                        if (string.IsNullOrWhiteSpace(item.Contact_t.StreetLine1))
                        {
                            sb.Append("[StreetLine1] ");
                        }
                        if (string.IsNullOrWhiteSpace(item.Contact_t.State))
                        {
                            sb.Append("[State] ");
                        }
                        if (string.IsNullOrWhiteSpace(item.Contact_t.Zipcode))
                        {
                            sb.Append("[Zip] ");
                        }
                        if (string.IsNullOrWhiteSpace(item.Contact_t.Country))
                        {
                            sb.Append("[country] ");
                        }
                        if (sb.Length > 0)
                        {
                            Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "Billing address missing:" + sb.ToString());
                            continue;
                        }

                        sucessCount++;
                    }
                    _logger.InfoFormat("completed in {0} with total:{1} and fail:{2}",
                        (DateTime.Now - start),
                        totalCount,
                        totalCount - sucessCount);

                    _file.Dispose();
                    _file.Close();
                }
            }
            catch (Exception exp)
            {
                _logger.Error(exp.Message, exp);
                throw;
            }
        }

        private static CreateTokenResponse CreateToken(Profile profile, CreditCard card, Contact billing)
        {
            var service = new CybersourceTokenService();
            return service.Create(profile, card, billing);            
        }

        private static void Log(
            int customerId,
            int creditCardId,
            int creditCardTypeId,
            string expiration,
            bool status,
            string message=null,
            string reasonCode=null,
            string token=null)
        {
            var msg = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                customerId,
                creditCardId,
                creditCardTypeId,
                expiration,
                status,
                message,
                reasonCode,
                token);

            _file.WriteLine(msg);
            if (!status)
            {
                _logger.Error(msg);
            }
        }
    }
}
