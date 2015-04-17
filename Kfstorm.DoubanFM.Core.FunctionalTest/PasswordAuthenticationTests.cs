using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.FunctionalTest
{
    [TestFixture]
    public class PasswordAuthenticationTests
    {
        private static void ValidateUserInfo(UserInfo userInfo)
        {
            Assert.IsNotNull(userInfo);
            Assert.IsNotEmpty(userInfo.AccessToken);
            Assert.IsNotEmpty(userInfo.RefreshToken);
            Assert.IsNotEmpty(userInfo.Username);
            Assert.Greater(userInfo.ExpiresIn, 0);
            Assert.Greater(userInfo.UserId, 0);
        }

        [Test]
        public async void TestLogOnSuccess_MailAddress()
        {
            var authentication = new PasswordAuthentication(Generator.ServerConnection)
            {
                Username = Generator.MailAddress,
                Password = Generator.Password
            };
            var userInfo = await authentication.Authenticate();
            ValidateUserInfo(userInfo);
        }

        [Test]
        public async void TestLogOnSuccess_UpperCaseMailAddress()
        {
            var authentication = new PasswordAuthentication(Generator.ServerConnection)
            {
                Username = Generator.MailAddress.ToUpper(),
                Password = Generator.Password
            };
            var userInfo = await authentication.Authenticate();
            ValidateUserInfo(userInfo);
        }

        [Test]
        public async void TestLogOnSuccess_Username()
        {
            var authentication = new PasswordAuthentication(Generator.ServerConnection)
            {
                Username = Generator.Username,
                Password = Generator.Password
            };
            var userInfo = await authentication.Authenticate();
            ValidateUserInfo(userInfo);
        }
    }
}