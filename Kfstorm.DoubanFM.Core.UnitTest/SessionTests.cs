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
        public IServerConnection BasicServerConnectionMock;
        public IAuthentication BasicAuthenticationMock;

        public UserInfo UserInfoExample = new UserInfo
        {
            AccessToken = "12345678",
            RefreshToken = "87654321",
            Username = "TestUser",
            ExpiresIn = 12345678,
            UserId = 12345,
        };

        public SessionTests()
        {
            var mock = new Mock<IAuthentication>();
            mock.Setup(a => a.Authenticate()).ReturnsAsync(UserInfoExample);
            BasicAuthenticationMock = mock.Object;

            BasicServerConnectionMock = new Mock<IServerConnection>().Object;
        }

        [Test]
        public async void TestLogOnSuccess()
        {
            var session = new Session(BasicServerConnectionMock);
            Assert.IsNull(session.UserInfo);
            await session.LogOn(BasicAuthenticationMock);
            Assert.IsNotNull(session.UserInfo);
            Assert.IsNotEmpty(session.UserInfo.AccessToken);
            Assert.IsNotEmpty(session.UserInfo.RefreshToken);
            Assert.IsNotEmpty(session.UserInfo.Username);
            Assert.AreNotEqual(0, session.UserInfo.ExpiresIn);
            Assert.AreNotEqual(0, session.UserInfo.UserId);
        }

        [Test]
        public void TestLogOnFailure()
        {
            var authenticationMock = new Mock<IAuthentication>();
            authenticationMock.Setup(a => a.Authenticate()).ThrowsAsync(new Exception("Test failure."));
            var session = new Session(BasicServerConnectionMock);
            Assert.IsNull(session.UserInfo);
            var ex = Assert.Throws<AggregateException>(() => session.LogOn(authenticationMock.Object).Wait()).InnerException;
            Assert.AreEqual("Test failure.", ex.Message);
            Assert.IsNull(session.UserInfo);
        }

        [Test]
        public void TestLogOnFailure_Exception()
        {
            var authenticationMock = new Mock<IAuthentication>();
            authenticationMock.Setup(a => a.Authenticate()).Throws(new Exception("Test failure."));
            var session = new Session(BasicServerConnectionMock);
            Assert.IsNull(session.UserInfo);
            var ex = Assert.Throws<AggregateException>(() => session.LogOn(authenticationMock.Object).Wait()).InnerException;
            Assert.AreEqual("Test failure.", ex.Message);
            Assert.IsNull(session.UserInfo);
        }

        [Test]
        public async void TestLogOffSuccess()
        {
            var session = new Session(BasicServerConnectionMock);
            Assert.IsNull(session.UserInfo);
            await session.LogOn(BasicAuthenticationMock);
            Assert.IsNotNull(session.UserInfo);
            session.LogOff();
            Assert.IsNull(session.UserInfo);
        }

        [Test]
        public async void TestAlreadyLoggedOn()
        {
            var session = new Session(BasicServerConnectionMock);
            await session.LogOn(BasicAuthenticationMock);
            Assert.IsNotNull(session.UserInfo);
            Assert.That(() => session.LogOn(BasicAuthenticationMock).Wait(), Throws.InnerException.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void TestAlreadyLoggedOff()
        {
            var session = new Session(BasicServerConnectionMock);
            Assert.IsNull(session.UserInfo);
            Assert.That(() => session.LogOff(), Throws.InvalidOperationException);
        }

        [Test]
        public async void TestChangeSessionStateWhenLoggingOn()
        {
            var authenticationMock = new Mock<IAuthentication>();
            var signal = new ManualResetEvent(false);
            authenticationMock.Setup(a => a.Authenticate()).Returns(Task.Run(() =>
            {
                signal.WaitOne();
                return Task.FromResult(UserInfoExample);
            }));
            var session = new Session(BasicServerConnectionMock);
            Assert.IsNull(session.UserInfo);
            var task = session.LogOn(authenticationMock.Object);
            Assert.That(() => session.LogOn(authenticationMock.Object).Wait(), Throws.InnerException.TypeOf<InvalidOperationException>());
            Assert.That(() => session.LogOff(), Throws.InvalidOperationException);
            Assert.IsNull(session.UserInfo);
            signal.Set();
            await task;
            Assert.IsNotNull(session.UserInfo);
        }
    }
}
