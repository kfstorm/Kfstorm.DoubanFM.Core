using Moq;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class SessionTests
    {
        [Test]
        public async void TestLogOnSuccess()
        {
            var authenticationMock = new Mock<IAuthentication>();
            authenticationMock.Setup(a => a.Authenticate()).ReturnsAsync(new LogOnResult
            {
                UserInfo = new UserInfo
                {
                    AccessToken = "12345678",
                    RefreshToken = "87654321",
                    Username = "TestUser",
                    ExpiresIn = 12345678,
                    UserId = 12345,
                }
            });
            var session = new Session();
            Assert.AreEqual(SessionState.LoggedOff, session.State);
            Assert.IsNull(session.UserInfo);
            var success = await session.LogOn(authenticationMock.Object);
            Assert.IsTrue(success);
            Assert.AreEqual(SessionState.LoggedOn, session.State);
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
            authenticationMock.Setup(a => a.Authenticate()).ReturnsAsync(new LogOnResult { ErrorMessage = "Test error message." });
            var session = new Session();
            Assert.AreEqual(SessionState.LoggedOff, session.State);
            Assert.IsNull(session.UserInfo);
            var success = await session.LogOn(authenticationMock.Object);
            Assert.IsFalse(success);
            Assert.AreEqual(SessionState.LoggedOff, session.State);
            Assert.IsNull(session.UserInfo);
        }
    }
}
