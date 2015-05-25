using System;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.FunctionalTest
{
    [TestFixture]
    public class ServerBugTests
    {
        [Description("Issue #16: Search channel return count can be less than the parameter setting")]
        [TestCase("阿兰", 20)]
        [TestCase("Taylor Swift", 100)]
        [TestCase("周杰伦", 20)]
        public async void TestSearchChannelIgnoresMaxSize(string query, int maxSize)
        {
            var discovery = Generator.Discovery;
            var start = 0;
            var total = 0;
            while (true)
            {
                var channels = await discovery.SearchChannel(query, start, maxSize);
                Assert.IsNotNull(channels);
                Assert.IsNotNull(channels.CurrentList);
                Assert.IsNotNull(channels.TotalCount);
                total += channels.CurrentList.Count;
                var expectedSize = Math.Min(channels.TotalCount.Value - total, maxSize);
                if (channels.CurrentList.Count < expectedSize)
                {
                    // Case found
                    Assert.Pass($"Expected size: {expectedSize}. Actual size: {channels.CurrentList.Count}");
                }
                if (total >= channels.TotalCount)
                {
                    break;
                }
            }
            Assert.Fail("Case not found");
        }

        [Description("Issue #17: GetOfflineRedHeartSongs ignores parameter 'maxSize'")]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async void TestGetOfflineRedHeartSongsIgnoresMaxSize(int maxSize)
        {
            var discovery = Generator.Discovery;
            await discovery.Session.LogOn(new PasswordAuthentication(discovery.ServerConnection)
            {
                Username = Generator.Username,
                Password = Generator.Password
            });
            var songs = await discovery.GetOfflineRedHeartSongs(maxSize, null);
            Assert.Greater(songs.Length, maxSize);
        }
    }
}
