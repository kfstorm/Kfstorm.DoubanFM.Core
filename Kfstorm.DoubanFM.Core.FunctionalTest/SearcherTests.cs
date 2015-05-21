using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.FunctionalTest
{
    [TestFixture]
    public class SearcherTests
    {
        [TestCase("阿兰", 20)]
        [TestCase("Taylor Swift", 100)]
        [TestCase("周杰伦", 20)]
        public async void TestSearchChannel(string query, int size)
        {
            var player = Generator.Player;
            var searcher = new Searcher(new ServerConnection());
            var start = 0;
            var allChannels = new List<Channel>();
            while (true)
            {
                var channels = await searcher.SearchChannel(query, start, size);
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
            var searcher = new Searcher(new ServerConnection());
            var channels = await searcher.SearchChannel(query, start, size);
            Assert.IsNotNull(channels);
            Assert.IsEmpty(channels);
        }
    }
}
