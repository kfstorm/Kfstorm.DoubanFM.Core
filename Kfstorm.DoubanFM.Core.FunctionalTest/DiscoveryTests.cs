using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.FunctionalTest
{
    [TestFixture]
    public class DiscoveryTests
    {
        [TestCase("阿兰", 20)]
        [TestCase("Taylor Swift", 100)]
        [TestCase("周杰伦", 20)]
        public async void TestSearchChannel(string query, int size)
        {
            var player = Generator.Player;
            var discovery = new Discovery(player.ServerConnection);
            var start = 0;
            var allChannels = new List<Channel>();
            while (true)
            {
                var channels = await discovery.SearchChannel(query, start, size);
                Assert.IsNotNull(channels);
                if (channels.Length == 0) break;
                foreach (var channel in channels)
                {
                    Validator.ValidateChannel(channel);
                }
                allChannels.AddRange(channels);
                start += channels.Length;
            }
            Assert.IsNotEmpty(allChannels);

            var random = new Random();
            for (var i = 0; i < 5; ++i)
            {
                var channel = allChannels[random.Next(allChannels.Count)];
                await player.ChangeChannel(channel);
                Validator.ValidateSong(player.CurrentSong);
            }
        }

        [TestCase("Some query that doesn't have any result", 0, 20)]
        [TestCase("Taylor Swift", 0, 0)]
        [TestCase("Taylor Swift", 0, -5)]
        [TestCase("Taylor Swift", -5, 0)]
        [TestCase("Taylor Swift", -5, 5)]
        public async void TestSearchChannelWithoutResult(string query, int start, int size)
        {
            var discovery = new Discovery(new ServerConnection());
            var channels = await discovery.SearchChannel(query, start, size);
            Assert.IsNotNull(channels);
            Assert.IsEmpty(channels);
        }

        [TestCase("Jessie J")]
        [TestCase("Adele")]
        public async void TestGetSongDetail(string query)
        {
            var player = Generator.Player;
            var discovery = new Discovery(player.ServerConnection);
            var channels = await discovery.SearchChannel(query, 0, 1);
            Assert.IsNotNull(channels);
            Assert.AreEqual(1, channels.Length);
            var channel = channels[0];
            Validator.ValidateChannel(channel);
            await player.ChangeChannel(channel);
            Validator.ValidateSong(player.CurrentSong);
            var songDetail = await discovery.GetSongDetail(player.CurrentSong.Sid);
            Assert.IsNotNull(songDetail);
            Assert.IsNotEmpty(songDetail.ArtistChannels);
            foreach (var artistChannel in songDetail.ArtistChannels)
            {
                Validator.ValidateChannel(artistChannel);
            }

            Assert.IsNotNull(songDetail.SimilarSongChannel);
        }

        [TestCase("陈奕迅")]
        [TestCase("刘德华")]
        public async void TestGetChannelInfo(string query)
        {
            var player = Generator.Player;
            var discovery = new Discovery(player.ServerConnection);
            var channels = await discovery.SearchChannel(query, 0, 1);
            Assert.IsNotNull(channels);
            Assert.AreEqual(1, channels.Length);
            var channel = channels[0];
            Validator.ValidateChannel(channel);
            await player.ChangeChannel(channel);
            Validator.ValidateSong(player.CurrentSong);
            var songDetail = await discovery.GetSongDetail(player.CurrentSong.Sid);
            Assert.IsNotNull(songDetail);
            Assert.IsNotNull(songDetail.SimilarSongChannel);
            channel = await discovery.GetChannelInfo(songDetail.SimilarSongChannel.Value);
            Validator.ValidateChannel(channel);
            Assert.AreEqual(songDetail.SimilarSongChannel.Value, channel.Id);

            Assert.IsNotEmpty(songDetail.ArtistChannels);
            foreach (var artistChannel in songDetail.ArtistChannels)
            {
                var newChannel = await discovery.GetChannelInfo(artistChannel.Id);
                Validator.ValidateChannel(newChannel);
                Assert.AreEqual(artistChannel.Id, newChannel.Id);
                Assert.AreEqual(artistChannel.Name, newChannel.Name);
                Assert.AreEqual(artistChannel.CoverUrl, newChannel.CoverUrl);
                Assert.AreEqual(artistChannel.Description, newChannel.Description);
            }
        }

        [TestCase("1638676", "9dd2", true)]
        [TestCase("153931", "4886", true)]
        [TestCase("544342", "c34c", true)]
        [TestCase("1888024", "5676", true)]
        [TestCase("1509097", "a8c6", false)]
        [TestCase("1382374", "d906", false)]
        public async void TestGetLyrics(string sid, string ssid, bool hasLyrics)
        {
            var discovery = new Discovery(Generator.ServerConnection);
            var lyrics = await discovery.GetLyrics(sid, ssid);
            if (hasLyrics)
            {
                Assert.IsNotNull(lyrics);
                Assert.IsNotEmpty(lyrics);
            }
            else
            {
                Assert.IsNull(lyrics);
            }
        }
    }
}
