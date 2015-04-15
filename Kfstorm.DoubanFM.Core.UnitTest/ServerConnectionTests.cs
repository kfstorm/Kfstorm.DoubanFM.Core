using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class ServerConnectionTests
    {
        [Test]
        public void TestReadWriteContext()
        {
            var serverConnection = new ServerConnection();
            var properties = typeof(IServerConnection).GetProperties().Where(p => p.Name != "Context").ToList();
            foreach (var property in properties)
            {
                var value = GetTestValue(property);
                property.SetValue(serverConnection, value);
                Assert.AreEqual(value, property.GetValue(serverConnection));
            }
            Assert.AreEqual(properties.Count, serverConnection.Context.Count);
        }

        [Test]
        public async void TestServerException_Get()
        {
            var serverConnection = new Mock<ServerConnection> {CallBase = true};
            var requestMock = new Mock<HttpWebRequest>();
            var responseMock = new Mock<HttpWebResponse>();
            requestMock.Setup(r => r.GetResponseAsync()).ThrowsAsync(new WebException("Test message", null, WebExceptionStatus.ProtocolError, responseMock.Object));
            responseMock.Setup(r => r.GetResponseStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes(Resource.ErrorResponseSample)));
            serverConnection.Protected().Setup<HttpWebRequest>("CreateRequest", ItExpr.IsAny<Uri>()).Returns(requestMock.Object);

            var ex = await AssertEx.ThrowsAsync<ServerException>(async () => await serverConnection.Object.Get(new Uri("http://anyUri.com")));
            Assert.IsNotNull(ex);
            Assert.AreEqual(123, ex.Code);
            Assert.IsNotEmpty(ex.ErrorMessage);
        }

        [Test]
        public async void TestServerException_Get_OldApi()
        {
            var serverConnection = new Mock<ServerConnection> { CallBase = true };
            var requestMock = new Mock<HttpWebRequest>();
            var responseMock = new Mock<HttpWebResponse>();
            requestMock.Setup(r => r.GetResponseAsync()).ReturnsAsync(responseMock.Object);
            responseMock.Setup(r => r.GetResponseStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes(Resource.ErrorResponseSample_OldApi)));
            serverConnection.Protected().Setup<HttpWebRequest>("CreateRequest", ItExpr.IsAny<Uri>()).Returns(requestMock.Object);

            var ex = await AssertEx.ThrowsAsync<ServerException>(async () => await serverConnection.Object.Get(new Uri("http://anyUri.com")));
            Assert.IsNotNull(ex);
            Assert.AreEqual(123, ex.Code);
            Assert.IsNotEmpty(ex.ErrorMessage);
        }

        [Test]
        public async void TestServerException_Post()
        {
            var serverConnection = new Mock<ServerConnection> { CallBase = true };
            var requestMock = new Mock<HttpWebRequest>();
            var responseMock = new Mock<HttpWebResponse>();
            requestMock.Setup(r => r.GetResponseAsync()).ThrowsAsync(new WebException("Test message", null, WebExceptionStatus.ProtocolError, responseMock.Object));
            responseMock.Setup(r => r.GetResponseStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes(Resource.ErrorResponseSample)));
            serverConnection.Protected().Setup<HttpWebRequest>("CreateRequest", ItExpr.IsAny<Uri>()).Returns(requestMock.Object);

            var ex = await AssertEx.ThrowsAsync<ServerException>(async () => await serverConnection.Object.Get(new Uri("http://anyUri.com")));
            Assert.IsNotNull(ex);
            Assert.AreEqual(123, ex.Code);
            Assert.IsNotEmpty(ex.ErrorMessage);
        }

        [Test]
        public async void TestServerException_Post_OldApi()
        {
            var serverConnection = new Mock<ServerConnection> { CallBase = true };
            var requestMock = new Mock<HttpWebRequest>();
            var responseMock = new Mock<HttpWebResponse>();
            requestMock.Setup(r => r.GetResponseAsync()).ReturnsAsync(responseMock.Object);
            responseMock.Setup(r => r.GetResponseStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes(Resource.ErrorResponseSample_OldApi)));
            serverConnection.Protected().Setup<HttpWebRequest>("CreateRequest", ItExpr.IsAny<Uri>()).Returns(requestMock.Object);

            var ex = await AssertEx.ThrowsAsync<ServerException>(async () => await serverConnection.Object.Get(new Uri("http://anyUri.com")));
            Assert.IsNotNull(ex);
            Assert.AreEqual(123, ex.Code);
            Assert.IsNotEmpty(ex.ErrorMessage);
        }

        private object GetTestValue(PropertyInfo property)
        {
            if (property.PropertyType == typeof(string))
            {
                return property.Name + "String";
            }
            if (property.PropertyType == typeof(Uri))
            {
                return new Uri($"http://www.{property.Name}.com");
            }
            throw new NotSupportedException();
        }
    }
}
