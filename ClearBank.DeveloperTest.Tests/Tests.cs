using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Services;
using NUnit.Framework;
using Moq;
using ClearBank.DeveloperTest.Data;

namespace ClearBank.DeveloperTest.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void PaymentService_MakeChapsPayment_Success()
        {
            // Arrange
            var mockDataStore = new Mock<IDataStore>(MockBehavior.Strict);
            mockDataStore.Setup(p => p.GetAccount("222")).Returns(new Account
            {
                AccountNumber = "222",
                Balance = 300m,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
            });
            mockDataStore.Setup(p => p.GetAccount("111")).Returns(new Account
            {
                AccountNumber = "111",
                Balance = 200m,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
            });


            var ps = new PaymentService(mockDataStore.Object);

            var mpr = new MakePaymentRequest() {
                CreditorAccountNumber = "111",
                DebitorAccountNumber = "222",
                Amount = 100m,
                PaymentDate = DateTime.Now,
                PaymentScheme = PaymentScheme.Chaps
            };


            // Act
            var result = ps.MakePayment(mpr);

            // Assert
            Assert.AreEqual(true, result.Success);
        }

        [Test]
        public void PaymentService_MakeFasterPaymentWithInsufficientFunds_Fail()
        {
            // Arrange
            var mockDataStore = new Mock<IDataStore>(MockBehavior.Strict);
            mockDataStore.Setup(p => p.GetAccount("222")).Returns(new Account
            {
                AccountNumber = "222",
                Balance = 100m,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
            });
            mockDataStore.Setup(p => p.GetAccount("111")).Returns(new Account
            {
                AccountNumber = "111",
                Balance = 200m,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
            });


            var ps = new PaymentService(mockDataStore.Object);

            var mpr = new MakePaymentRequest()
            {
                CreditorAccountNumber = "111",
                DebitorAccountNumber = "222",
                Amount = 300m,
                PaymentDate = DateTime.Now,
                PaymentScheme = PaymentScheme.Chaps
            };


            // Act
            var result = ps.MakePayment(mpr);

            // Assert
            Assert.AreEqual(false, result.Success);
        }
    }
}
