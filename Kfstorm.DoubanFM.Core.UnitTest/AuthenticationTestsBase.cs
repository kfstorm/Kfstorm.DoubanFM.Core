using System;
using System.Collections.Generic;
using Moq;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    public abstract class AuthenticationTestsBase
    {
        protected Mock<IServerConnection> CreateServerConnectionMockWithContext()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Context).Returns(new Dictionary<string, string>
            {
                { "client_id", "testClientId12345" },
                { "client_secret", "testClientSecret12345" },
                { "redirect_uri", "http://www.testRedirectUri.com" },
            });
            serverConnectionMock.Setup(s => s.ClientId).Returns("testClientId12345");
            serverConnectionMock.Setup(s => s.ClientSecret).Returns("testClientSecret12345");
            serverConnectionMock.Setup(s => s.RedirectUri).Returns(new Uri("http://www.testRedirectUri.com"));
            return serverConnectionMock;
        }
    }
}
