using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.FunctionalTest
{
    [TestFixture]
    public class PlayerTests
    {
        private IPlayer _player;
        private IPlayer _playerWithUser;

        [TestFixtureSetUp]
        public void SetUp()
        {
            Initialize().Wait();
        }

        private async Task Initialize()
        {
            _player = Generator.Player;
            _playerWithUser = Generator.Player;
            await _playerWithUser.Session.LogOn(new PasswordAuthentication(_playerWithUser.ServerConnection)
            {
                Username = Generator.MailAddress,
                Password = Generator.Password
            });
            await _player.RefreshChannelList();
            await _playerWithUser.RefreshChannelList();
        }

        private IPlayer GetPlayer(bool loggedOn)
        {
            return loggedOn ? _playerWithUser : _player;
        }

        [TestCase(true)]
        [TestCase(false)]
        public async void TestInitialize(bool loggedOn)
        {
            var player = Generator.Player;
            if (loggedOn)
            {
                await player.Session.LogOn(new PasswordAuthentication(player.ServerConnection)
                {
                    Username = Generator.MailAddress,
                    Password = Generator.Password
                });
            }
            await player.RefreshChannelList();
            ValidateChannelList(player.ChannelList);
            Assert.IsNull(player.CurrentChannel);
            Assert.IsNull(player.CurrentSong);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Description("If user didn't log in, then some channels always return the same songs. #1")]
        public async void TestNoDuplicateSongs(bool loggedOn)
        {
            var player = GetPlayer(loggedOn);
            await player.ChangeChannel(player.ChannelList.ChannelGroups[0].Channels[0]);
            var count = 0;
            var sidCollection = new HashSet<string>();
            while (count <= 10)
            {
                ++count;
                Assert.IsTrue(sidCollection.Add(player.CurrentSong.Sid), $"Detected duplicate sid {player.CurrentSong.Sid} after skiped {count} times.");
                await player.Next(NextCommandType.SkipCurrentSong);
            }
        }

        [Test]
        [Description("Throws exception when songs in channel is empty #6")]
        public async void TestNoServerExceptionWhenChannelIsEmpty()
        {
            var player = GetPlayer(false);
            await AssertEx.ThrowsAsync<NoAvailableSongsException>(async () => await player.ChangeChannel(new Channel(-3))); // 红心 channel is empty for anonymous users.
        }

        [TestCase(false)]
        [TestCase(true)]
        public async void TestChangeChannel(bool loggedOn)
        {
            var player = GetPlayer(loggedOn);
            await player.ChangeChannel(null);
            var channels = player.ChannelList.ChannelGroups.SelectMany(group => group.Channels).Distinct().Where(channel => channel.Id != -3);
            channels = channels.Take(3);
            foreach (var channel in channels)
            {
                var oldChannel = player.CurrentChannel;
                Assert.AreNotEqual(oldChannel, channel);
                await player.ChangeChannel(channel);
                Assert.AreEqual(channel, player.CurrentChannel);
                ValidateChannel(player.CurrentChannel);
                ValidateSong(player.CurrentSong);
            }
        }

        [TestCase(false, NextCommandType.SkipCurrentSong)]
        [TestCase(false, NextCommandType.CurrentSongEnded)]
        [TestCase(false, NextCommandType.BanCurrentSong)]
        [TestCase(true, NextCommandType.SkipCurrentSong)]
        [TestCase(true, NextCommandType.CurrentSongEnded)]
        [TestCase(true, NextCommandType.BanCurrentSong)]
        public async void TestNext(bool loggedOn, NextCommandType type)
        {
            var player = GetPlayer(loggedOn);
            await player.ChangeChannel(new Channel(0));
            for (var i = 0; i < 10; ++i)
            {
                var originalSong = player.CurrentSong;
                await player.Next(type);
                ValidateSong(player.CurrentSong);
                Assert.AreNotEqual(originalSong, player.CurrentSong);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async void TestSetRedHeart(bool loggedOn)
        {
            var player = GetPlayer(loggedOn);
            await player.ChangeChannel(new Channel(0));
            Assert.IsNotNull(player.CurrentSong);
            var originalStatus = player.CurrentSong.Like;
            await player.SetRedHeart(originalStatus);
            Assert.AreEqual(originalStatus, player.CurrentSong.Like);
            await player.SetRedHeart(!originalStatus);
            Assert.AreEqual(!originalStatus, player.CurrentSong.Like);
            await player.SetRedHeart(!originalStatus);
            Assert.AreEqual(!originalStatus, player.CurrentSong.Like);
        }

        private void ValidateChannelList(ChannelList channalList)
        {
            Assert.IsNotNull(channalList);
            Assert.IsNotEmpty(channalList.ChannelGroups);
            CollectionAssert.AllItemsAreUnique(channalList.ChannelGroups.Select(group => group.GroupName));
            CollectionAssert.AllItemsAreUnique(channalList.ChannelGroups.Select(group => group.GroupId));
            foreach (var group in channalList.ChannelGroups)
            {
                Assert.IsNotNull(group);
                Assert.IsNotEmpty(group.GroupName);
                Assert.IsNotEmpty(group.Channels);
                foreach (var channel in group.Channels)
                {
                    ValidateChannel(channel);
                }
            }
            var channels = channalList.ChannelGroups.SelectMany(group => group.Channels).ToList();
            Assert.IsTrue(channels.Any(channel => !string.IsNullOrEmpty(channel.Description)));
            Assert.IsTrue(channels.Any(channel => channel.SongCount > 0));
        }

        private void ValidateChannel(Channel channel)
        {
            Assert.IsNotNull(channel);
            Assert.IsNotEmpty(channel.Name);
            Assert.IsNotEmpty(channel.CoverUrl);
            if (channel.Name != "私人")
            {
                Assert.AreNotEqual(0, channel.Id);
            }
            else
            {
                Assert.AreEqual(0, channel.Id);
            }
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
        }
    }
}
