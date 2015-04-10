using System;
using System.Linq;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class GetPlayListTypeTests
    {
        [Test]
        public void TestConvertPlayListTypeEnumToString()
        {
            var enumValues = Enum.GetValues(typeof(GetPlayListType)).Cast<GetPlayListType>();
            foreach (var type in enumValues)
            {
                var stringValue = GetPlayListTypeString.GetString(type);
                Assert.IsNotEmpty(stringValue);
                Assert.AreEqual(1, stringValue.Length);
            }
        }
    }
}
