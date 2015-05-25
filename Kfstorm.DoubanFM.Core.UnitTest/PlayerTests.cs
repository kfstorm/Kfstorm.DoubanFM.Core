using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public async void TestChangeChannel()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("app_channels")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.ChannelListExample).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.PlayList).Verifiable();
            var session = new Session(serverConnectionMock.Object);
            var player = new Player(session);
            var channelGroups = await new Discovery(session).GetRecommendedChannels();
            Assert.IsNotNull(channelGroups);
            var newChannels = channelGroups.SelectMany(group => group.Channels).ToArray();
            Assert.IsNotEmpty(newChannels);
            foreach (var newChannel in newChannels)
            {
                await player.ChangeChannel(newChannel);
                serverConnectionMock.Verify();
                Assert.AreEqual(newChannel, player.CurrentChannel);
                Validator.ValidateChannel(player.CurrentChannel);
                Validator.ValidateSong(player.CurrentSong);
            }
        }

        [TestCase(ChangeChannelCommandType.Normal)]
        [TestCase(ChangeChannelCommandType.PlayRelatedSongs)]
        public async void TestChangeChannelAndValidateParameterStart(ChangeChannelCommandType type)
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.PlayList).Verifiable();
            var session = new Session(serverConnectionMock.Object);
            var player = new Player(session);
            var channel = new Channel(123) { Start = "StartCode" };
            await player.ChangeChannel(channel, type);
            serverConnectionMock.Verify();
            var startValidator = new Func<Uri, bool>(uri =>
            {
                var queries = uri.GetQueries();
                switch (type)
                {
                    case ChangeChannelCommandType.Normal:
                        return queries.ContainsKey("start") && queries["start"] == "StartCode";
                    case ChangeChannelCommandType.PlayRelatedSongs:
                        return !queries.ContainsKey("start");
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            });
            serverConnectionMock.Verify(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && startValidator(u)), It.IsAny<Action<HttpWebRequest>>()));
        }

        private readonly JObject _defaultPlayList = JObject.Parse(Resource.PlayList);

        [TestCase(NextCommandType.SkipCurrentSong)]
        [TestCase(NextCommandType.BanCurrentSong)]
        [TestCase(NextCommandType.CurrentSongEnded)]
        public async void TestNext(NextCommandType type)
        {
            var playList = _defaultPlayList.DeepClone();
            string reportType = GetReportTypeStringByNextCommandType(type);

            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("app_channels")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.ChannelListExample).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist")
                                                                  && (u.GetQueries().Contains(new KeyValuePair<string, string>("type", "n"))
                                                                      || u.GetQueries().Contains(new KeyValuePair<string, string>("type", reportType)))
                                                                  && !u.GetQueries().ContainsKey("start")),
                It.IsAny<Action<HttpWebRequest>>())).Returns(() => Task.FromResult(playList.ToString())).Callback(
                    () =>
                    {
                        if (type == NextCommandType.CurrentSongEnded)
                        {
                            playList["song"] = new JArray();
                        }
                        else
                        {
                            var first = playList["song"][0];
                            first.Remove();
                            playList["song"].Last.AddAfterSelf(first);
                        }
                    }).Verifiable();
            var session = new Session(serverConnectionMock.Object);
            var player = new Player(session);
            await player.ChangeChannel(new Channel(0));

            var originalSong = player.CurrentSong;
            for (var i = 0; i < 5; ++i)
            {
                await player.Next(type);
                var newSong = player.CurrentSong;
                Assert.AreNotEqual(newSong, originalSong);
                originalSong = newSong;
            }
            serverConnectionMock.Verify();
        }

        private static string GetReportTypeStringByNextCommandType(NextCommandType type)
        {
            string reportType;
            switch (type)
            {
                case NextCommandType.SkipCurrentSong:
                    reportType = "s";
                    break;
                case NextCommandType.BanCurrentSong:
                    reportType = "b";
                    break;
                case NextCommandType.CurrentSongEnded:
                    reportType = "e";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            return reportType;
        }

        [TestCase(true)]
        [TestCase(false)]
        public async void TestSetRedHeart(bool redHeart)
        {
            string reportType = GetReportTypeStringByRedHeart(redHeart);
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("app_channels")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.ChannelListExample).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist")
                                                                  && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "n"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.PlayList).Verifiable();
            var session = new Session(serverConnectionMock.Object);
            var player = new Player(session);
            await player.ChangeChannel(new Channel(0));

            var originalSong = player.CurrentSong;
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist")
                                                                  && u.GetQueries().Contains(new KeyValuePair<string, string>("type", reportType))
                                                                  && u.GetQueries().Contains(new KeyValuePair<string, string>("sid", player.CurrentSong.Sid))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.PlayList).Verifiable();
            await player.SetRedHeart(redHeart);
            Assert.AreEqual(originalSong, player.CurrentSong);
            serverConnectionMock.Verify();
        }

        private static string GetReportTypeStringByRedHeart(bool redHeart)
        {
            return redHeart ? "r" : "u";
        }

        [Test]
        public async void TestSongEnded()
        {
            var playList = _defaultPlayList.DeepClone();
            var emptyPlayList = _defaultPlayList.DeepClone();
            emptyPlayList["song"] = new JArray();
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("app_channels")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.ChannelListExample).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "n"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(playList.ToString()).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "p"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(playList.ToString()).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "e"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(emptyPlayList.ToString()).Verifiable();

            var session = new Session(serverConnectionMock.Object);
            var player = new Player(session);
            await player.ChangeChannel(new Channel(0));
            var songCount = playList["song"].Count();
            for (var i = 0; i < songCount*2; ++i)
            {
                await player.Next(NextCommandType.CurrentSongEnded);
            }
            serverConnectionMock.Verify();
        }

        [Test]
        public async void TestChannelNotSelectedException()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            var session = new Session(serverConnectionMock.Object);
            var player = new Player(session);
            await AssertEx.ThrowsAsync<ChannelNotSelectedException>(async () => await player.Next(NextCommandType.SkipCurrentSong));
        }

        [Test]
        public async void TestNoAvailableSongsThenRecover()
        {
            var playList = _defaultPlayList.DeepClone();
            var emptyPlayList = _defaultPlayList.DeepClone();
            emptyPlayList["song"] = new JArray();
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "n"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(playList.ToString()).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "p"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(playList.ToString()).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "s"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(emptyPlayList.ToString()).Verifiable();

            var session = new Session(serverConnectionMock.Object);
            var player = new Player(session);
            await player.ChangeChannel(new Channel(0));
            await AssertEx.ThrowsAsync<NoAvailableSongsException>(async () => await player.Next(NextCommandType.SkipCurrentSong));
            Assert.IsNotNull(player.CurrentChannel);
            Assert.IsNull(player.CurrentSong);
            await player.Next(NextCommandType.SkipCurrentSong);
            Assert.IsNotNull(player.CurrentSong);
        }

        [Test]
        public async void TestSongNotSelectedException()
        {
            var playList = _defaultPlayList.DeepClone();
            var emptyPlayList = _defaultPlayList.DeepClone();
            emptyPlayList["song"] = new JArray();
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "n"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(playList.ToString()).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "p"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(playList.ToString()).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "s"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(emptyPlayList.ToString()).Verifiable();

            var session = new Session(serverConnectionMock.Object);
            var player = new Player(session);
            await player.ChangeChannel(new Channel(0));
            await AssertEx.ThrowsAsync<NoAvailableSongsException>(async () => await player.Next(NextCommandType.SkipCurrentSong));
            Assert.IsNotNull(player.CurrentChannel);
            Assert.IsNull(player.CurrentSong);
            await AssertEx.ThrowsAsync<SongNotSelectedException>(async () => await player.SetRedHeart(true));
        }

        [Test]
        public async void TestNoAvailableSongs_ChangeChannel()
        {
            var emptyPlayList = _defaultPlayList.DeepClone();
            emptyPlayList["song"] = new JArray();
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("app_channels")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.ChannelListExample).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "n"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(emptyPlayList.ToString()).Verifiable();

            var session = new Session(serverConnectionMock.Object);
            var player = new Player(session);
            await AssertEx.ThrowsAsync<NoAvailableSongsException>(async () => await player.ChangeChannel(new Channel(0)));
            serverConnectionMock.Verify();
        }

        [TestCase(NextCommandType.SkipCurrentSong)]
        [TestCase(NextCommandType.BanCurrentSong)]
        public async void TestNoAvailableSongs_Next(NextCommandType type)
        {
            var emptyPlayList = _defaultPlayList.DeepClone();
            emptyPlayList["song"] = new JArray();
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("app_channels")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.ChannelListExample).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "n"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.PlayList).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", GetReportTypeStringByNextCommandType(type)))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(emptyPlayList.ToString()).Verifiable();

            var player = new Player(new Session(serverConnectionMock.Object));
            await player.ChangeChannel(new Channel(0));
            await AssertEx.ThrowsAsync<NoAvailableSongsException>(async () => await player.Next(type));
            serverConnectionMock.Verify();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async void TestNoAvailableSongs_SetRedHeart(bool redHeart)
        {
            var emptyPlayList = _defaultPlayList.DeepClone();
            emptyPlayList["song"] = new JArray();
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("app_channels")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.ChannelListExample).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", "n"))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.PlayList).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist") && u.GetQueries().Contains(new KeyValuePair<string, string>("type", GetReportTypeStringByRedHeart(redHeart)))),
                It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(emptyPlayList.ToString()).Verifiable();

            var player = new Player(new Session(serverConnectionMock.Object));
            await player.ChangeChannel(new Channel(0));
            await AssertEx.ThrowsAsync<NoAvailableSongsException>(async () => await player.SetRedHeart(redHeart));
            serverConnectionMock.Verify();
        }
    }
}
