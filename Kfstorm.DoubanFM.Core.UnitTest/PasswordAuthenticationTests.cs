using System;
using Moq;
using Newtonsoft.Json;
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
        public async void TestAuthenticateFailure_EmptyUsername()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var pAuth = new PasswordAuthentication(serverConnectionMock.Object)
            {
                Password = "anyPassword",
            };
            await AssertEx.ThrowsAsync<InvalidOperationException>(async () => await pAuth.Authenticate());
        }

        [Test]
        public async void TestAuthenticateFailure_EmptyPassword()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var pAuth = new PasswordAuthentication(serverConnectionMock.Object)
            {
                Username = "anyUsername",
            };
            await AssertEx.ThrowsAsync<InvalidOperationException>(async () => await pAuth.Authenticate());
        }

        [Test]
        public async void TestAuthenticateFailure_InvalidJson()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var expectedPostUri = new Uri("https://www.douban.com/service/auth2/token");
            serverConnectionMock.Setup(s => s.Post(It.Is<Uri>(u => u == expectedPostUri), It.Is<byte[]>(d => d.Length > 0))).ReturnsAsync("###").Verifiable();
            var pAuth = new PasswordAuthentication(serverConnectionMock.Object)
            {
                Username = "anyUsername",
                Password = "anyPassword",
            };
            await AssertEx.ThrowsAsync<JsonReaderException>(async () => await pAuth.Authenticate());
            serverConnectionMock.Verify();
        }

        [Test]
        public async void TestAuthenticateFailure_Exception()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var expectedPostUri = new Uri("https://www.douban.com/service/auth2/token");
            serverConnectionMock.Setup(s => s.Post(It.Is<Uri>(u => u == expectedPostUri), It.Is<byte[]>(d => d.Length > 0))).ThrowsAsync(new Exception("Test message.")).Verifiable();
            var pAuth = new PasswordAuthentication(serverConnectionMock.Object)
            {
                Username = "anyUsername",
                Password = "anyPassword",
            };
            var ex = await AssertEx.ThrowsAsync<Exception>(async () => await pAuth.Authenticate());
            Assert.AreEqual("Test message.", ex.Message);
            serverConnectionMock.Verify();
        }
    }
}
