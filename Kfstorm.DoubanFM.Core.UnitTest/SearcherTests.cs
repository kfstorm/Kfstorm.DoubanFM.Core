using System;
using System.Net;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class SearcherTests
    {
        [Test]
        public async void TestSearchChannel()
        {
            var emptySearchChannelResult = JObject.Parse(Resource.SearchChannelResultExample).DeepClone();
            emptySearchChannelResult["channels"] = null;
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("search/channel") && int.Parse(u.GetQueries()["start"]) < 100), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.SearchChannelResultExample).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("search/channel") && int.Parse(u.GetQueries()["start"]) >= 100), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(emptySearchChannelResult.ToString()).Verifiable();

            var searcher = new Searcher(serverConnectionMock.Object);
            var start = 0;
            var limit = 20;
            while (true)
            {
                var channels = await searcher.SearchChannel("any text here", start, limit);
                Assert.IsNotNull(channels);
                if (start < 100) Assert.IsNotEmpty(channels);
                foreach (var channel in channels)
                {
                    Validator.ValidateChannel(channel);
                }
                if (channels.Length == 0) break;
                start += limit;
            }
            serverConnectionMock.Verify();
        }
    }
}
