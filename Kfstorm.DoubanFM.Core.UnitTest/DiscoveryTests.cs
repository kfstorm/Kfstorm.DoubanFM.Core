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
        public async void TestGetRecommendedChannels()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.IsAny<Uri>(), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.ChannelListExample).Verifiable();
            var session = new Session(serverConnectionMock.Object);
            var discovery = new Discovery(session);
            var channelGroups = await discovery.GetRecommendedChannels();
            serverConnectionMock.Verify();
            Assert.IsNotNull(channelGroups);
            Assert.AreEqual(4, channelGroups.Length);
            for (int i = 0; i < channelGroups.Length; ++i)
            {
                var channelGroup = channelGroups[i];
                Assert.IsNotEmpty(channelGroup.GroupName);
                Assert.IsNotEmpty(channelGroup.Channels);
                if (i > 0)
                {
                    Assert.Greater(channelGroups[i].GroupId, channelGroups[i - 1].GroupId);
                }
                foreach (var channel in channelGroup.Channels)
                {
                    Validator.ValidateChannel(channel);
                }
            }
        }

        [Test]
        public async void TestSearchChannel()
        {
            var emptySearchChannelResult = JObject.Parse(Resource.SearchChannelResultExample).DeepClone();
            emptySearchChannelResult["channels"] = null;
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("search/channel") && int.Parse(u.GetQueries()["start"]) < 100), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.SearchChannelResultExample).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("search/channel") && int.Parse(u.GetQueries()["start"]) >= 100), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(emptySearchChannelResult.ToString()).Verifiable();

            var discovery = new Discovery(new Session(serverConnectionMock.Object));
            var start = 0;
            var limit = 20;
            while (true)
            {
                var channels = await discovery.SearchChannel("any text here", start, limit);
                Assert.IsNotNull(channels);
                Assert.Greater(channels.TotalCount, 0);
                if (start < 100) Assert.IsNotEmpty(channels.CurrentList);
                foreach (var channel in channels.CurrentList)
                {
                    Validator.ValidateChannel(channel);
                }
                if (channels.CurrentList.Count == 0) break;
                start += limit;
            }
            serverConnectionMock.Verify();
        }

        [Test]
        public async void TestGetSongDetail()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("song_detail")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.SongDetailExample).Verifiable();

            var discovery = new Discovery(new Session(serverConnectionMock.Object));
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

            var discovery = new Discovery(new Session(serverConnectionMock.Object));
            var channel = await discovery.GetChannelInfo(123);
            Validator.ValidateChannel(channel);
            serverConnectionMock.Verify();
        }

        [Test]
        public async void TestGetLyrics()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("lyric")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.LyricsExample).Verifiable();

            var discovery = new Discovery(new Session(serverConnectionMock.Object));
            var lyrics = await discovery.GetLyrics("123", "12345");
            Assert.IsNotNull(lyrics);
            Assert.IsNotEmpty(lyrics);
        }

        [Test]
        public async void TestGetOfflineRedHeartSongs()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.PlayList).Verifiable();
            var session = new Session(serverConnectionMock.Object);
            var discovery = new Discovery(session);
            var songs = await discovery.GetOfflineRedHeartSongs(20, null);
            Assert.IsNotNull(songs);
            Assert.IsNotEmpty(songs);
            foreach (var song in songs)
            {
                Validator.ValidateSong(song);
            }
        }

        [Test]
        public async void TestUpdateSongUrl()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.PlayList).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("song_url")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.SongUrlExample).Verifiable();
            var session = new Session(serverConnectionMock.Object);
            var discovery = new Discovery(session);
            var songs = await discovery.GetOfflineRedHeartSongs(20, null);
            Assert.IsNotNull(songs);
            Assert.IsNotEmpty(songs);
            var song = songs[0];
            var oldUrl = song.Url;
            await discovery.UpdateSongUrl(song);
            Assert.IsNotNull(song.Url);
            Assert.IsNotEmpty(song.Url);
            Assert.AreNotEqual(oldUrl, song.Url);
        }
    }
}
