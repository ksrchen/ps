using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ps.domain;

namespace ps.test
{
    [TestClass]
    public class CybersourceTokenServiceTest
    {
        [TestMethod]
        public void UpdateTokenTest()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", ".\\App_Data");
            var token = CreateToken();

            var profileDomain = new ProfileDomain();
            var profile = profileDomain.Get(1);

            var service = new CybersourceTokenService();
            var result = service.Update(profile, token, new models.CreditCard
            {
                    ExpirationMonth = "01",
                    ExpirationYear = "2019",
            });

            Assert.IsTrue(result.Status);

            var detail = service.Get(profile, token);
            Assert.IsTrue(detail.Status);
            Assert.AreEqual(detail.CreditCard.ExpirationMonth, "01");
            Assert.AreEqual(detail.CreditCard.ExpirationYear, "2019");

            DeleteToken(token);
        }

        [TestMethod]
        public void DeleteTokenTest()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", ".\\App_Data");
            var token = CreateToken();

            var profileDomain = new ProfileDomain();
            var profile = profileDomain.Get(1);

            var service = new CybersourceTokenService();
            var result = service.Delete(profile, token);
            Assert.IsTrue(result.Status);

        }
        [TestMethod]
        public void CreateTokenTest()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", ".\\App_Data");

            var profileDomain = new ProfileDomain();
            var profile = profileDomain.Get(1);

            var service = new CybersourceTokenService();
            var result = service.Create(profile, new models.CreditCard()
            {
                CardType = "001",
                CardNumber = "4111111111111111",
                ExpirationMonth = "01",
                ExpirationYear = "2019"
            },
            new models.Contact
            {
                FirstName = "john",
                LastName = "doe",
                City = "glendora",
                Country ="US",
                EmailAddress = "foo@bar.com",
                PostalCode = "91789",
                State = "CA",
                StreetLine1 = "foo street",
            });

            Assert.IsTrue(result.Status);

            DeleteToken(result.Token);

        }

        private string CreateToken()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", ".\\App_Data");

            var profileDomain = new ProfileDomain();
            var profile = profileDomain.Get(1);

            var service = new CybersourceTokenService();
            var result = service.Create(profile, new models.CreditCard()
            {
                CardType = "001",
                CardNumber = "4111111111111111",
                ExpirationMonth = "01",
                ExpirationYear = "2019"
            },
            new models.Contact
            {
                FirstName = "john",
                LastName = "doe",
                City = "glendora",
                Country = "US",
                EmailAddress = "foo@bar.com",
                PostalCode = "91789",
                State = "CA",
                StreetLine1 = "foo street",
            });

            return result.Token;
        }
        
        private void DeleteToken(string token)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", ".\\App_Data");

            var profileDomain = new ProfileDomain();
            var profile = profileDomain.Get(1);

            var service = new CybersourceTokenService();
            var result = service.Delete(profile, token);
            Assert.IsTrue(result.Status);

        }
    }
}
