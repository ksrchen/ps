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

                    var query = db.CCInfo_t.AsNoTracking().Include("Contact_t").Where(p => p.Active);                    
                    long totalCount = query.LongCount();

                    _logger.InfoFormat("Found {0} credit card(s) ", totalCount);

                    int i = 1;
                    foreach (var item in query)
                    {
                        _logger.InfoFormat("processing {0} of {1}", i++, totalCount);
                        StringBuilder sb = new StringBuilder();

                        string cardNumber = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.CCNum))
                        {
                            sb.Append("Card number is missing. ");
                        }
                        cardNumber = LNEncryption.LNDecrypt(item.CCNum);

                        string expiryMonth = string.Empty;
                        string expiryYear = string.Empty;

                        if (string.IsNullOrWhiteSpace(item.CCExpDate))
                        {
                            sb.Append("Expiration date missing." );
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
                                sb.Append("Invalid expiration date. ");
                            }
                        }
                        int month = 0;
                        int year = 0;
                        if (!int.TryParse(expiryMonth, out month))
                        {
                            sb.Append("Invalid month. ");
                        }

                        if (!int.TryParse(expiryYear, out year))
                        {
                            sb.Append("Invalid year."); 
                        }
                        year = year + 2000;

                        if (new DateTime(year, month, 1) < DateTime.Now.Date)
                        {
                            sb.Append("Credit card expired. ");
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
                                sb.Append("Invalid credit card type. ");
                                break;
                        }

                        //try to get email from cc contact, customer current billing contact or  customer contact
                        string email = item.Contact_t.EmailAddress;
                        if (string.IsNullOrWhiteSpace(item.Contact_t.EmailAddress))
                        {
                            email = item.Customer_t.Contact_t.EmailAddress; //billing contact
                            if (string.IsNullOrWhiteSpace(email))
                            {
                                email = item.Customer_t.Contact_t1.EmailAddress; //contact
                            }
                        }
                        var ccContact = GetContact(item.Contact_t);  //try contact attached to cc first
                        var ccError = CheckContact(ccContact);
                        if (!string.IsNullOrWhiteSpace(ccError))
                        {
                            var billingContact = GetContact(item.Customer_t.Contact_t);
                            var msg = CheckContact(billingContact);

                            if (string.IsNullOrWhiteSpace(msg))
                            {
                                ccContact = billingContact;  //use customer current billing contact instead
                                ccError = string.Empty;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(ccError))
                        {
                            sb.Append(ccError);
                        }

                        if (sb.Length > 0)
                        {
                            Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, sb.ToString());
                            continue;
                        }

                        ccContact.EmailAddress = email;
                        var card = new CreditCard()
                        {
                            CardNumber = cardNumber,
                            CardType = creditCardType,
                            ExpirationMonth = month.ToString("00"),
                            ExpirationYear = year.ToString("0000")
                        };

                        var response = CreateToken(profile, card, ccContact, (int)item.CustomerID);
                        if (!response.Status)
                        {
                            Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, response.Message, response.ReasonCode);
                            continue;
                        }

                        //save token
                        if (!SaveToken(db, item.CCInfoID, response.Token))
                        {
                            Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "failed to save token", response.ReasonCode, response.Token);
                            continue;
                        }

                        //log succesful token
                        Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, true, response.Message, response.ReasonCode, response.Token);
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

        private static bool SaveToken(LARSEntities db, decimal p1, string p2)
        {
            var result = db.Database.ExecuteSqlCommand("UPDATE CCInfo_t SET Token = {0} where CCInfoID = {1}", p2, p1);
            return result > 0;
        }
        private static string CheckContact(Contact contact)
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(contact.FirstName))
            {
                sb.Append("FirstName is missing. ");
            }
            if (string.IsNullOrWhiteSpace(contact.LastName))
            {
                sb.Append("LastName is missing ");
            }
            if (string.IsNullOrWhiteSpace(contact.City))
            {
                sb.Append("City is missing. ");
            }
            if (string.IsNullOrWhiteSpace(contact.StreetLine1))
            {
                sb.Append("StreetLine is missing. ");
            }
            if (string.IsNullOrWhiteSpace(contact.State))
            {
                sb.Append("State is missing. ");
            }
            if (string.IsNullOrWhiteSpace(contact.PostalCode))
            {
                sb.Append("Zip is missing. ");
            }
            if (string.IsNullOrWhiteSpace(contact.Country))
            {
                sb.Append("Country is missing. ");
            }

            return sb.ToString();

        }
        private static Contact GetContact(Contact_t fromContact)
        {
            return new Contact()
            {
                FirstName = fromContact.FirstName,
                LastName = fromContact.LastName,
                City = fromContact.City,
                Country = fromContact.Country,
                PostalCode = fromContact.Zipcode,
                State = fromContact.State,
                StreetLine1 = fromContact.StreetLine1
            };

        }
        private static CreateTokenResponse CreateToken(Profile profile, CreditCard card, Contact billing, int customerId)
        {
            var service = new CybersourceTokenService();
            var resp =  service.Create(profile, card, billing, string.Format("BID:{0}", customerId));
            if (resp.Status)
            {
                resp.Token = string.Format("p{0}-{1}", profile.ProfileID, resp.Token);
            }
            return resp;
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
            var msg = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8:MM/dd hh:mm:ss}",
                customerId,
                creditCardId,
                creditCardTypeId,
                expiration,
                status,
                message,
                reasonCode,
                token,
                DateTime.Now);

            _file.WriteLine(msg);
            if (!status)
            {
                _logger.Error(msg);
            }
        }
    }
}
