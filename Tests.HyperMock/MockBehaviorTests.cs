using System;
using System.Linq;
using HyperMock;
#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using Tests.HyperMock.Support;

namespace Tests.HyperMock
{
    [TestClass]
    public class MockBehaviorTests
    {
        [TestMethod]
        public void DefaultMockBehaviorIsLoose()
        {
            Mock.DefaultMockBehavior = MockBehavior.Loose;

            var mock = Mock.Create<IAccountService>();

            Assert.AreEqual(MockBehavior.Loose, mock.MockBehavior);
        }

        [TestMethod]
        public void DefaultMockBehaviorIsStrict()
        {
            Mock.DefaultMockBehavior = MockBehavior.Strict;

            var mock = Mock.Create<IAccountService>();

            Assert.AreEqual(MockBehavior.Strict, mock.MockBehavior);
        }

        [TestMethod]
        public void LooseBehaviorReturnDefaultValue()
        {
            var mock = Mock.Create<IAccountService>();
            mock.MockBehavior = MockBehavior.Loose;

            var result = mock.Object.CanDebit("123", 2);

            Assert.AreEqual(false, result);
        }

#if WINDOWS_UWP
        [TestMethod]
        public void MockBehaviorIsSettable()
        {
            var mock = Mock.Create<IAccountService>();
            mock.MockBehavior = MockBehavior.Loose;

            var result = mock.Object.CanDebit("123", 2);

            Assert.AreEqual(false, result);

            mock.MockBehavior = MockBehavior.Strict;

            Assert.ThrowsException<InvalidOperationException>(() => mock.Object.CanDebit("123", 2));
        }

        [TestMethod]
        public void StrictBehaviorThrowsException()
        {
            var mock = Mock.Create<IAccountService>();
            mock.MockBehavior = MockBehavior.Strict;

            Assert.ThrowsException<InvalidOperationException>(() => mock.Object.CanDebit("123", 2));
        }
#else
        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void MockBehaviorIsSettable()
        {
            var mock = Mock.Create<IAccountService>();
            mock.MockBehavior = MockBehavior.Loose;

            var result = mock.Object.CanDebit("123", 2);

            Assert.AreEqual(false, result);

            mock.MockBehavior = MockBehavior.Strict;

            mock.Object.CanDebit("123", 2);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void StrictBehaviorThrowsException()
        {
            var mock = Mock.Create<IAccountService>();
            mock.MockBehavior = MockBehavior.Strict;

            mock.Object.CanDebit("123", 2);
        }
#endif
        [TestCleanup]
        public void Cleanup()
        {
            Mock.DefaultMockBehavior = MockBehavior.Loose;
        }
    }
}
