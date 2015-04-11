using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class OAuthAuthenticationTests
    {
        private Mock<IServerConnection> CreateServerConnectionMockWithContext()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Context).Returns(new Dictionary<string, string>
            {
                { "client_id", "testClientId12345" },
                { "client_secret", "testClientSecret12345" }
            });
            return serverConnectionMock;
        }

        [Test]
        public async void TestAuthenticateSuccess()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var expectedPostUri = new Uri("https://www.douban.com/service/auth2/token?client_id=testClientId12345&client_secret=testClientSecret12345&redirect_uri=http%3A%2F%2Fwww.testredirecturi.com%2F&grant_type=authorization_code&code=testCode");
            serverConnectionMock.Setup(s => s.Post(It.Is<Uri>(u=>u == expectedPostUri), null)).ReturnsAsync(Resource.TestOAuthResponse).Verifiable();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object, new Uri("http://www.testRedirectUri.com"))
            {
                GetRedirectedUri = uri => Task.FromResult(new Uri("http://www.testRedirectUri.com?code=testCode"))
            };
            var result = await oAuth.Authenticate();
            serverConnectionMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.UserInfo);
            Assert.IsNotNull(result.UserInfo.AccessToken);
            Assert.IsNotNull(result.UserInfo.RefreshToken);
            Assert.IsNotNull(result.UserInfo.Username);
            Assert.AreNotEqual(0, result.UserInfo.UserId);
            Assert.AreNotEqual(0, result.UserInfo.ExpiresIn);
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAuthorizationCode_ErrorMessage()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object, new Uri("http://www.testRedirectUri.com"))
            {
                GetRedirectedUri = uri => Task.FromResult(new Uri("http://www.testRedirectUri.com?error=access_denied"))
            };
            var result = await oAuth.Authenticate();
            Assert.IsNotNull(result);
            Assert.IsNull(result.UserInfo);
            Assert.AreEqual(-1, result.ErrorCode);
            Assert.AreEqual("access_denied", result.ErrorMessage);
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAuthorizationCode_NullResponseUri()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object, new Uri("http://www.testRedirectUri.com"))
            {
                GetRedirectedUri = uri => Task.FromResult((Uri)null)
            };
            var result = await oAuth.Authenticate();
            Assert.IsNotNull(result);
            Assert.IsNull(result.UserInfo);
            Assert.AreEqual(-1, result.ErrorCode);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAuthorizationCode_UnknownResponseUri()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object, new Uri("http://www.testRedirectUri.com"))
            {
                GetRedirectedUri = uri => Task.FromResult(new Uri("http://www.unknown.com"))
            };
            var result = await oAuth.Authenticate();
            Assert.IsNotNull(result);
            Assert.IsNull(result.UserInfo);
            Assert.AreEqual(-1, result.ErrorCode);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAuthorizationCode_Exception()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object, new Uri("http://www.testRedirectUri.com"))
            {
                GetRedirectedUri = async uri => await Task.Run(new Func<Task<Uri>>(() => { throw new Exception("Test message."); })),
            };
            var result = await oAuth.Authenticate();
            Assert.IsNotNull(result);
            Assert.IsNull(result.UserInfo);
            Assert.AreEqual(-1, result.ErrorCode);
            Assert.AreEqual("Test message.", result.ErrorMessage);
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAccessToken_InvalidJson()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            serverConnectionMock.Setup(s => s.Post(It.IsAny<Uri>(), null)).ReturnsAsync("###").Verifiable();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object, new Uri("http://www.testRedirectUri.com"))
            {
                GetRedirectedUri = uri => Task.FromResult(new Uri("http://www.testRedirectUri.com?code=testCode"))
            };
            var result = await oAuth.Authenticate();
            serverConnectionMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.UserInfo);
            Assert.AreEqual(-1, result.ErrorCode);
            Assert.IsNotEmpty(result.ErrorMessage);
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAccessToken_Exception()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            serverConnectionMock.Setup(s => s.Post(It.IsAny<Uri>(), null)).ThrowsAsync(new Exception("Test message.")).Verifiable();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object, new Uri("http://www.testRedirectUri.com"))
            {
                GetRedirectedUri = uri => Task.FromResult(new Uri("http://www.testRedirectUri.com?code=testCode"))
            };
            var result = await oAuth.Authenticate();
            serverConnectionMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNull(result.UserInfo);
            Assert.AreEqual(-1, result.ErrorCode);
            Assert.AreEqual("Test message.", result.ErrorMessage);
        }

        [Test]
        public async void TestUnAuthenticateSuccess()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object, null);
            Assert.IsNull(await oAuth.UnAuthenticate());
        }
    }
}
