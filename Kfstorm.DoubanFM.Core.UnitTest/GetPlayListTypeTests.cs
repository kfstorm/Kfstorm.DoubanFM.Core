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
            var enumValues = Enum.GetValues(typeof(ReportType)).Cast<ReportType>();
            foreach (var type in enumValues)
            {
                var stringValue = ReportTypeString.GetString(type);
                Assert.IsNotEmpty(stringValue);
                Assert.AreEqual(1, stringValue.Length);
            }
        }

        [Test]
        public void TestInvalidEnum()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() => ReportTypeString.GetString((ReportType)int.MaxValue));
        }
    }
}
