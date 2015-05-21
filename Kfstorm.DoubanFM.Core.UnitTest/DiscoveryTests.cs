using System;
using System.Net;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class DiscoveryTests
    {
        [Test]
        public async void TestSearchChannel()
        {
            var emptySearchChannelResult = JObject.Parse(Resource.SearchChannelResultExample).DeepClone();
            emptySearchChannelResult["channels"] = null;
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("search/channel") && int.Parse(u.GetQueries()["start"]) < 100), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.SearchChannelResultExample).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("search/channel") && int.Parse(u.GetQueries()["start"]) >= 100), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(emptySearchChannelResult.ToString()).Verifiable();

            var discovery = new Discovery(serverConnectionMock.Object);
            var start = 0;
            var limit = 20;
            while (true)
            {
                var channels = await discovery.SearchChannel("any text here", start, limit);
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

        [Test]
        public async void TestGetSongDetail()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("song_detail")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.SongDetailExample).Verifiable();

            var discovery = new Discovery(serverConnectionMock.Object);
            var songDetail = await discovery.GetSongDetail("12345");
            Assert.IsNotNull(songDetail);
            Assert.IsNotEmpty(songDetail.ArtistChannels);
            foreach (var channel in songDetail.ArtistChannels)
            {
                Validator.ValidateChannel(channel);
            }
            Assert.IsTrue(songDetail.SimilarSongChannel.HasValue && songDetail.SimilarSongChannel.Value > 0);
            serverConnectionMock.Verify();
        }

        [Test]
        public async void TestGetChannelInfo()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("channel_info")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.ChannelInfoExample).Verifiable();

            var discovery = new Discovery(serverConnectionMock.Object);
            var channel = await discovery.GetChannelInfo(123);
            Validator.ValidateChannel(channel);
            serverConnectionMock.Verify();
        }
    }
}
