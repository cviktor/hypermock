﻿using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HyperMock.Windows.Examples
{
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<IAccountService> _mockService;
        private AccountController _controller;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _mockService = Mock.Create<IAccountService>();
            _controller = new AccountController(_mockService.Object);
        }

        [TestMethod]
        public void CreditAddsToAccount()
        {
            var info = new AccountInfo { Number = "1234", CreditAmount = 100 };

            _controller.Credit(info);

            _mockService.Verify(s => s.Credit(info.Number, info.CreditAmount), Occurred.Once());
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void CreditWithInvalidAmountThrowsException()
        {
            var info = new AccountInfo { Number = "1234", CreditAmount = -100 };
            _mockService.Setup(s => s.Credit(info.Number, Param.Is<int>(p => p < 1))).Throws(new NotSupportedException());

            _controller.Credit(info);
        }

        [TestMethod]
        public void CreditFailsWithUnknownAmount()
        {
            var info = new AccountInfo { Number = "1234", CreditAmount = 100 };

            _controller.Credit(info);

            _mockService.Verify(s => s.Credit(info.Number, 200), Occurred.Never());
        }

        [TestMethod]
        public void CreditWithAnyAmount()
        {
            var info = new AccountInfo { Number = "1234", CreditAmount = 100 };

            _controller.Credit(info);

            _mockService.Verify(s => s.Credit(info.Number, Param.IsAny<int>()), Occurred.Once());
        }

        [TestMethod]
        public void CreditWithAmountAboveMin()
        {
            var info = new AccountInfo { Number = "1234", CreditAmount = 2 };

            _controller.Credit(info);

            _mockService.Verify(s => s.Credit(info.Number, Param.Is<int>(p => p > 1)), Occurred.Once());
        }

        [TestMethod]
        public void CreditFailsWithAmountBelowMin()
        {
            var info = new AccountInfo { Number = "1234", CreditAmount = 1 };

            _controller.Credit(info);

            _mockService.Verify(s => s.Credit(info.Number, Param.Is<int>(p => p > 1)), Occurred.Never());
        }

        [TestMethod]
        public void DebitCorrectAccount()
        {
            var info1 = new AccountInfo { Number = "1234", DebitAmount = 100 };
            var info2 = new AccountInfo { Number = "4321", DebitAmount = 50 };
            _mockService.Setup(s => s.CanDebit(info1.Number, info1.DebitAmount)).Returns(false);
            _mockService.Setup(s => s.CanDebit(info2.Number, info2.DebitAmount)).Returns(true);

            _controller.Debit(info2);

            _mockService.Verify(s => s.Debit(info2.Number, info2.DebitAmount), Occurred.Once());
            _mockService.Verify(s => s.Debit(info1.Number, info1.DebitAmount), Occurred.Never());
        }

        [TestMethod]
        public void DebitCorrectAccountMatchingRegex()
        {
            var info1 = new AccountInfo { Number = "1234", DebitAmount = 100 };
            var info2 = new AccountInfo { Number = "12345678", DebitAmount = 50 };
            _mockService.Setup(s => s.CanDebit(Param.IsRegex("^[0-9]{4}$"), info1.DebitAmount)).Returns(true);

            _controller.Debit(info1);

            _mockService.Verify(s => s.Debit(info1.Number, info1.DebitAmount), Occurred.Once());
            _mockService.Verify(s => s.Debit(info2.Number, info2.DebitAmount), Occurred.Never());
        }

        [TestMethod]
        public void HasAccounts()
        {
            _mockService.SetupGet(s => s.HasAccounts).Returns(true);

            var result = _controller.HasAccounts();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasNoAccounts()
        {
            _mockService.SetupGet(s => s.HasAccounts).Returns(false);

            var result = _controller.HasAccounts();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ManageDisablesAccount()
        {
            _controller.Manage(false);

            _mockService.VerifySet(s => s.HasAccounts, false, Occurred.Once());
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ManageDisablesAccountThrowsException()
        {
            _mockService.SetupSet(s => s.HasAccounts).SetValue(false).Throws<NotSupportedException>();

            _controller.Manage(false);
        }

        [TestMethod]
        public async Task DownloadStatementsAsyncReturnsStatement()
        {
            var info = new AccountInfo { Number = "1234" };

            _mockService.Setup(s => s.DownloadStatementsAsync("1234")).Returns(Task.Run(() => "Statement"));

            var statement = await _controller.DownloadStatementsAsync(info);

            Assert.AreEqual("Statement", statement);
        }
    }
}
