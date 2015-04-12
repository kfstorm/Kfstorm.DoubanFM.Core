using System;
using System.Linq;
using System.Net;
using Moq;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public async void TestInitialization()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.IsAny<Uri>(), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.ChannelListExample).Verifiable();
            var sessionMock = new Mock<ISession>();
            var player = new Player(serverConnectionMock.Object, sessionMock.Object);
            Assert.IsNull(player.ChannelList);
            await player.Initialize();
            serverConnectionMock.Verify();
            Assert.IsNotNull(player.ChannelList);
            var channelList = player.ChannelList;
            Assert.IsNotNull(channelList.ChannelGroups);
            Assert.AreEqual(4, channelList.ChannelGroups.Length);
            for (int i = 0; i < channelList.ChannelGroups.Length; ++i)
            {
                var channelGroup = channelList.ChannelGroups[i];
                Assert.IsNotEmpty(channelGroup.GroupName);
                Assert.IsNotEmpty(channelGroup.Channels);
                if (i > 0)
                {
                    Assert.Greater(channelList.ChannelGroups[i].GroupId, channelList.ChannelGroups[i - 1].GroupId);
                }
                foreach (var channel in channelGroup.Channels)
                {
                    ValidateChannel(channel);
                }
            }
        }

        [Test]
        public async void TestChangeChannel()
        {
            var serverConnectionMock = new Mock<IServerConnection>();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("app_channels")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.ChannelListExample).Verifiable();
            serverConnectionMock.Setup(s => s.Get(It.Is<Uri>(u => u.AbsolutePath.EndsWith("playlist")), It.IsAny<Action<HttpWebRequest>>())).ReturnsAsync(Resource.PlayList).Verifiable();
            var sessionMock = new Mock<ISession>();
            var player = new Player(serverConnectionMock.Object, sessionMock.Object);
            await player.Initialize();
            Assert.IsNotNull(player.ChannelList);
            var newChannels = player.ChannelList.ChannelGroups.SelectMany(group => group.Channels).ToArray();
            foreach (var newChannel in newChannels)
            {
                await player.ChangeChannel(newChannel);
                serverConnectionMock.Verify();
                Assert.AreEqual(newChannel, player.CurrentChannel);
                ValidateChannel(player.CurrentChannel);
                ValidateSong(player.CurrentSong);
            }
        }

        private void ValidateChannel(Channel channel)
        {
            Assert.IsNotNull(channel);
            Assert.IsNotEmpty(channel.Name);
            Assert.IsNotEmpty(channel.Description);
            Assert.IsNotEmpty(channel.CoverUrl);
            if (channel.Name != "私人")
            {
                Assert.AreNotEqual(0, channel.Id);
            }
            else
            {
                Assert.AreEqual(0, channel.Id);
            }
            Assert.Greater(channel.SongCount, 0);
        }

        private void ValidateSong(Song song)
        {
            Assert.IsNotNull(song);
            Assert.IsNotEmpty(song.AlbumUrl);
            Assert.IsNotEmpty(song.PictureUrl);
            Assert.IsNotEmpty(song.Artist);
            Assert.IsNotEmpty(song.Url);
            Assert.IsNotEmpty(song.Title);
            Assert.AreNotEqual(0, song.Length);
            Assert.IsNotEmpty(song.Sid);
            Assert.IsNotEmpty(song.Aid);
            Assert.AreNotEqual(0, song.Kbps);
            Assert.IsNotEmpty(song.AlbumTitle);
            if (song.SubType != "T")
            {
                Assert.IsNotEmpty(song.Ssid);
                Assert.IsNotEmpty(song.Company);
                Assert.AreNotEqual(0, song.AverageRating);
                Assert.AreNotEqual(0, song.PublishTime);
                Assert.AreNotEqual(0, song.SongListsCount);
                Assert.IsNotEmpty(song.Sha256);
            }
        }
    }
}
