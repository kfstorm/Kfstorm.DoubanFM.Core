using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class SessionTests
    {
        public IAuthentication BasicAuthenticationMock;

        public LogOnResult SuccessLogOnResult = new LogOnResult
        {
            UserInfo = new UserInfo
            {
                AccessToken = "12345678",
                RefreshToken = "87654321",
                Username = "TestUser",
                ExpiresIn = 12345678,
                UserId = 12345,
            }
        };

        public LogOnResult FailureLogOnResult = new LogOnResult
        {
            ErrorCode = 1,
            ErrorMessage = "Test failure message."
        };

        public SessionTests()
        {
            var mock = new Mock<IAuthentication>();
            mock.Setup(a => a.Authenticate()).ReturnsAsync(SuccessLogOnResult);
            mock.Setup(a => a.UnAuthenticate()).ReturnsAsync(null);
            BasicAuthenticationMock = mock.Object;
        }

        [Test]
        public async void TestLogOnSuccess()
        {
            var session = new Session();
            Assert.IsNull(session.UserInfo);
            var error = await session.LogOn(BasicAuthenticationMock);
            Assert.IsNull(error);
            Assert.IsNotNull(session.UserInfo);
            Assert.IsNotEmpty(session.UserInfo.AccessToken);
            Assert.IsNotEmpty(session.UserInfo.RefreshToken);
            Assert.IsNotEmpty(session.UserInfo.Username);
            Assert.AreNotEqual(0, session.UserInfo.ExpiresIn);
            Assert.AreNotEqual(0, session.UserInfo.UserId);
        }

        [Test]
        public async void TestLogOnFailure()
        {
            var authenticationMock = new Mock<IAuthentication>();
            authenticationMock.Setup(a => a.Authenticate()).ReturnsAsync(FailureLogOnResult);
            var session = new Session();
            Assert.IsNull(session.UserInfo);
            var error = await session.LogOn(authenticationMock.Object);
            Assert.IsNotNull(error);
            Assert.IsNull(session.UserInfo);
        }

        [Test]
        public async void TestLogOnFailure_Exception()
        {
            var authenticationMock = new Mock<IAuthentication>();
            authenticationMock.Setup(a => a.Authenticate()).Throws(new Exception("Test failure."));
            var session = new Session();
            Assert.IsNull(session.UserInfo);
            var error = await session.LogOn(authenticationMock.Object);
            Assert.AreEqual("Test failure.", error);
            Assert.IsNull(session.UserInfo);
        }

        [Test]
        public async void TestLogOffSuccess()
        {
            var session = new Session();
            Assert.IsNull(session.UserInfo);
            var error = await session.LogOn(BasicAuthenticationMock);
            Assert.IsNull(error);
            Assert.IsNotNull(session.UserInfo);
            await session.LogOff(BasicAuthenticationMock);
            Assert.IsNull(session.UserInfo);
        }

        [Test]
        public async void TestLogOffFailure()
        {
            var authenticationMock = new Mock<IAuthentication>();
            authenticationMock.Setup(a => a.Authenticate()).ReturnsAsync(SuccessLogOnResult);
            authenticationMock.Setup(a => a.UnAuthenticate()).ReturnsAsync("Test failure.");
            var session = new Session();
            Assert.IsNull(session.UserInfo);
            var error = await session.LogOn(authenticationMock.Object);
            Assert.IsNull(error);
            Assert.IsNotNull(session.UserInfo);
            Assert.AreEqual("Test failure.", await session.LogOff(authenticationMock.Object));
            Assert.IsNotNull(session.UserInfo);
        }

        [Test]
        public async void TestLogOffFailure_Exception()
        {
            var authenticationMock = new Mock<IAuthentication>();
            authenticationMock.Setup(a => a.Authenticate()).ReturnsAsync(SuccessLogOnResult);
            authenticationMock.Setup(a => a.UnAuthenticate()).Throws(new Exception("Test failure."));
            var session = new Session();
            Assert.IsNull(session.UserInfo);
            var error = await session.LogOn(authenticationMock.Object);
            Assert.IsNull(error);
            Assert.IsNotNull(session.UserInfo);
            Assert.AreEqual("Test failure.", await session.LogOff(authenticationMock.Object));
            Assert.IsNotNull(session.UserInfo);
        }

        [Test]
        public async void TestAlreadyLoggedOn()
        {
            var session = new Session();
            var error = await session.LogOn(BasicAuthenticationMock);
            Assert.IsNull(error);
            Assert.IsNotNull(session.UserInfo);
            Assert.That(() => session.LogOn(BasicAuthenticationMock).Wait(), Throws.InnerException.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void TestAlreadyLoggedOff()
        {
            var session = new Session();
            Assert.IsNull(session.UserInfo);
            Assert.That(() => session.LogOff(BasicAuthenticationMock).Wait(), Throws.InnerException.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async void TestChangeSessionStateWhenLoggingOff()
        {
            var authenticationMock = new Mock<IAuthentication>();
            authenticationMock.Setup(a => a.Authenticate()).ReturnsAsync(SuccessLogOnResult);
            var signal = new ManualResetEvent(false);
            authenticationMock.Setup(a => a.UnAuthenticate()).Returns(Task.Run(() =>
            {
                signal.WaitOne();
                return (string)null;
            }));
            var session = new Session();
            var error = await session.LogOn(authenticationMock.Object);
            Assert.IsNull(error);
            Assert.IsNotNull(session.UserInfo);
            var task = session.LogOff(authenticationMock.Object);
            Assert.That(() => session.LogOn(authenticationMock.Object).Wait(), Throws.InnerException.TypeOf<InvalidOperationException>());
            Assert.That(() => session.LogOff(authenticationMock.Object).Wait(), Throws.InnerException.TypeOf<InvalidOperationException>());
            Assert.IsNotNull(session.UserInfo);
            signal.Set();
            Assert.IsNull(await task);
            Assert.IsNull(session.UserInfo);
        }

        [Test]
        public async void TestChangeSessionStateWhenLoggingOn()
        {
            var authenticationMock = new Mock<IAuthentication>();
            var signal = new ManualResetEvent(false);
            authenticationMock.Setup(a => a.Authenticate()).Returns(Task.Run(() =>
            {
                signal.WaitOne();
                return Task.FromResult(SuccessLogOnResult);
            }));
            var session = new Session();
            Assert.IsNull(session.UserInfo);
            var task = session.LogOn(authenticationMock.Object);
            Assert.That(() => session.LogOn(authenticationMock.Object).Wait(), Throws.InnerException.TypeOf<InvalidOperationException>());
            Assert.That(() => session.LogOff(authenticationMock.Object).Wait(), Throws.InnerException.TypeOf<InvalidOperationException>());
            Assert.IsNull(session.UserInfo);
            signal.Set();
            Assert.IsNull(await task);
            Assert.IsNotNull(session.UserInfo);
        }
    }
}
