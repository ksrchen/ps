using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ps.domain;

namespace ps.test
{
    [TestClass]
    public class CybersourcePaymentServiceTest
    {
        [TestMethod]
        public void TestSales()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", ".\\App_Data");

            var profileDomain = new ProfileDomain();
            var profile = profileDomain.Get(1);

            var service = new CybersourcePaymentService();
            var result = service.Sales(profile, "4018227272460176195662", new models.TranactionRequest
            {
                Amount = 9900,
                Currency = "USD",
                CustomerPO = "1234",
                ReferenceNumber = "11223333",
                PaymentToken = "4018227272460176195662",
                ShipFrom = new models.Contact
                {
                    PostalCode = "91789",
                },
                ShipTo = new models.Contact { PostalCode = "91789" },
                TaxAmount = 0,
                TransactionType = models.TransactionTypes.Sales,
                LineItems = new System.Collections.Generic.List<models.LineItem> { new models.LineItem {
                    CommodityCode = "foo",
                    ProductCode = "code 1",
                    ProductDescription = "test product",
                    ProductName = "test",
                    ProductSKU = "",
                    Quantity = 1,
                    TaxAmount = 0,
                    TaxRate = 1,
                    UnitPrice = 100,
                }}

            });

            Assert.IsTrue(result.Status);

        }

        [TestMethod]
        public void TestVoid()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", ".\\App_Data");

            var profileDomain = new ProfileDomain();
            var profile = profileDomain.Get(1);

            var service = new CybersourcePaymentService();
            var result = service.Void(profile, "4029352006890176056442", "1234");
            Assert.IsTrue(result.Status);

        }
        [TestMethod]
        public void TestRefund()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", ".\\App_Data");

            var profileDomain = new ProfileDomain();
            var profile = profileDomain.Get(1);

            var service = new CybersourcePaymentService();
            var result = service.Refund(profile, "4024127683710176056442", new models.TranactionRequest
            {
                Amount = 100,
                Currency = "USD",
                CustomerPO = "1234",
                ReferenceNumber = "11223333",
                PaymentToken = "4024127683710176056442",
                ShipFrom = new models.Contact
                {
                    PostalCode = "91789",
                },
                ShipTo = new models.Contact { PostalCode = "91789" },
                TaxAmount = 0,
                TransactionType = models.TransactionTypes.Sales,
                LineItems = new System.Collections.Generic.List<models.LineItem> { new models.LineItem {
                    CommodityCode = "foo",
                    ProductCode = "code 1",
                    ProductDescription = "test product",
                    ProductName = "test",
                    ProductSKU = "",
                    Quantity = 1,
                    TaxAmount = 0,
                    TaxRate = 1,
                    UnitPrice = 100,
                }}

            });

            Assert.IsTrue(result.Status);
        }
    }
}
