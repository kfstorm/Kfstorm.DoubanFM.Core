using System.Collections.Generic;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.FunctionalTest
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        [Description("If user didn't log in, then some channels always return the same songs. #1")]
        public async void TestNoDuplicateSongs()
        {
            var player = Generator.DefaultPlayer;
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
            var player = Generator.DefaultPlayer;
            await AssertEx.ThrowsAsync<NoAvailableSongsException>(async () => await player.ChangeChannel(new Channel(-3))); // 红心 channel is empty for anonymous users.
        }
    }
}
