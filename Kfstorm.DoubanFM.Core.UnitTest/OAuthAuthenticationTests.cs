using System;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class OAuthAuthenticationTests : AuthenticationTestsBase
    {
        [Test]
        public void TestGetRedirectUri()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object);
            Assert.AreEqual(serverConnectionMock.Object.RedirectUri, oAuth.RedirectUri);
        }

        [Test]
        public async void TestAuthenticateSuccess()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var expectedPostUri = new Uri("https://www.douban.com/service/auth2/token?client_id=testClientId12345&client_secret=testClientSecret12345&redirect_uri=http%3A%2F%2Fwww.testredirecturi.com%2F&grant_type=authorization_code&code=testCode");
            serverConnectionMock.Setup(s => s.Post(It.Is<Uri>(u=>u == expectedPostUri), null)).ReturnsAsync(Resource.TestOAuthResponse).Verifiable();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object)
            {
                GetRedirectedUri = uri => Task.FromResult(new Uri("http://www.testRedirectUri.com?code=testCode"))
            };
            var result = await oAuth.Authenticate();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AccessToken);
            Assert.IsNotNull(result.RefreshToken);
            Assert.IsNotNull(result.Username);
            Assert.AreNotEqual(0, result.UserId);
            Assert.AreNotEqual(0, result.ExpiresIn);
            serverConnectionMock.Verify();
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAuthorizationCode_ErrorMessage()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object)
            {
                GetRedirectedUri = uri => Task.FromResult(new Uri("http://www.testRedirectUri.com?error=access_denied"))
            };
            var ex = await AssertEx.ThrowsAsync<ServerException>(async () => await oAuth.Authenticate());
            Assert.IsNotNull(ex);
            Assert.AreEqual(-1, ex.Code);
            Assert.AreEqual("access_denied", ex.ErrorMessage);
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAuthorizationCode_NullResponseUri()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object)
            {
                GetRedirectedUri = uri => Task.FromResult((Uri)null)
            };
            await AssertEx.ThrowsAsync<OperationCanceledException>(async () => await oAuth.Authenticate());
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAuthorizationCode_UnknownResponseUri()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object)
            {
                GetRedirectedUri = uri => Task.FromResult(new Uri("http://www.unknown.com"))
            };
            await AssertEx.ThrowsAsync<OperationCanceledException>(async () => await oAuth.Authenticate());
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAuthorizationCode_Exception()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object)
            {
                GetRedirectedUri = async uri => await Task.Run(new Func<Task<Uri>>(() => { throw new Exception("Test message."); })),
            };
            var ex = await AssertEx.ThrowsAsync<Exception>(async () => await oAuth.Authenticate());
            Assert.AreEqual("Test message.", ex.Message);
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAccessToken_InvalidJson()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            serverConnectionMock.Setup(s => s.Post(It.IsAny<Uri>(), null)).ReturnsAsync("###").Verifiable();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object)
            {
                GetRedirectedUri = uri => Task.FromResult(new Uri("http://www.testRedirectUri.com?code=testCode"))
            };
            await AssertEx.ThrowsAsync<JsonReaderException>(async () => await oAuth.Authenticate());
            serverConnectionMock.Verify();
        }

        [Test]
        public async void TestAuthenticateFailure_FailedToGetAccessToken_Exception()
        {
            var serverConnectionMock = CreateServerConnectionMockWithContext();
            serverConnectionMock.Setup(s => s.Post(It.IsAny<Uri>(), null)).ThrowsAsync(new Exception("Test message.")).Verifiable();
            var oAuth = new OAuthAuthentication(serverConnectionMock.Object)
            {
                GetRedirectedUri = uri => Task.FromResult(new Uri("http://www.testRedirectUri.com?code=testCode"))
            };
            var ex = await AssertEx.ThrowsAsync<Exception>(async () => await oAuth.Authenticate());
            Assert.AreEqual("Test message.", ex.Message);
            serverConnectionMock.Verify();
        }
    }
}
