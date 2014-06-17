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
        private static StreamWriter _file;
        static void Main(string[] args)
        {
            _file =  new StreamWriter(@".\result.csv");
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
                var query = db.CCInfo_t.Include("Contact_t").Where(p => p.Active);
                foreach (var item in query)
                {
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
                    if (string.IsNullOrWhiteSpace(item.Contact_t.FirstName))
                    {
                        Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "First name is missing");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(item.Contact_t.LastName))
                    {
                        Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "Last name is missing");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(item.Contact_t.City))
                    {
                        Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "City is missing");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(item.Contact_t.StreetLine1))
                    {
                        Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "StreetLine1 is missing");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(item.Contact_t.State))
                    {
                        Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "State is missing");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(item.Contact_t.Zipcode))
                    {
                        Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "zip code is missing");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(item.Contact_t.Country))
                    {
                        Log((int)item.CustomerID, (int)item.CCInfoID, (int)item.CCTypeID, item.CCExpDate, false, "country is missing");
                        continue;
                    }
                }
                _file.Dispose();
                _file.Close();
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
            _file.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                customerId,
                creditCardId,
                creditCardTypeId,
                expiration,
                status,
                message,
                reasonCode,
                token);
        }
    }
}
