using System;
using Moq;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class PasswordAuthenticationTests : AuthenticationTestsBase
    {
        [Test]
        public async void TestAuthenticateSuccess()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var expectedPostUri = new Uri("https://www.douban.com/service/auth2/token");
            serverConnectionMock.Setup(s => s.Post(It.Is<Uri>(u => u == expectedPostUri), It.Is<byte[]>(d => d.Length > 0))).ReturnsAsync(Resource.TestOAuthResponse).Verifiable();
            var pAuth = new PasswordAuthentication(serverConnectionMock.Object)
            {
                Username = "anyUsername",
                Password = "anyPassword",
            };
            var result = await pAuth.Authenticate();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AccessToken);
            Assert.IsNotNull(result.RefreshToken);
            Assert.IsNotNull(result.Username);
            Assert.AreNotEqual(0, result.UserId);
            Assert.AreNotEqual(0, result.ExpiresIn);
            serverConnectionMock.Verify();
        }

        [Test]
        public void TestAuthenticateFailure_EmptyUsername()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var pAuth = new PasswordAuthentication(serverConnectionMock.Object)
            {
                Password = "anyPassword",
            };
            Assert.That(()=> pAuth.Authenticate().Wait(), Throws.InnerException.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void TestAuthenticateFailure_EmptyPassword()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var pAuth = new PasswordAuthentication(serverConnectionMock.Object)
            {
                Username = "anyUsername",
            };
            Assert.That(() => pAuth.Authenticate().Wait(), Throws.InnerException.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void TestAuthenticateFailure_InvalidJson()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var expectedPostUri = new Uri("https://www.douban.com/service/auth2/token");
            serverConnectionMock.Setup(s => s.Post(It.Is<Uri>(u => u == expectedPostUri), It.Is<byte[]>(d => d.Length > 0))).ReturnsAsync("###").Verifiable();
            var pAuth = new PasswordAuthentication(serverConnectionMock.Object)
            {
                Username = "anyUsername",
                Password = "anyPassword",
            };
            Assert.Throws<AggregateException>(() => pAuth.Authenticate().Wait());
            serverConnectionMock.Verify();
        }

        [Test]
        public void TestAuthenticateFailure_Exception()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var expectedPostUri = new Uri("https://www.douban.com/service/auth2/token");
            serverConnectionMock.Setup(s => s.Post(It.Is<Uri>(u => u == expectedPostUri), It.Is<byte[]>(d => d.Length > 0))).ThrowsAsync(new Exception("Test message.")).Verifiable();
            var pAuth = new PasswordAuthentication(serverConnectionMock.Object)
            {
                Username = "anyUsername",
                Password = "anyPassword",
            };
            var ex = Assert.Throws<AggregateException>(() => pAuth.Authenticate().Wait()).InnerException;
            Assert.AreEqual("Test message.", ex.Message);
            serverConnectionMock.Verify();
        }
    }
}
