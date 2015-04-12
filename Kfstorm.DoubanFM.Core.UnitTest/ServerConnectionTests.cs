using System;
using System.Linq;
using System.Reflection;
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
