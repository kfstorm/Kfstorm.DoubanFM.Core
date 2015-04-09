using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    [TestFixture]
    public class ChannelListTests
    {
        [Test]
        public void TestNewChannelList()
        {
            var jsonContent = Resource.ChannelListExample;
            var channelList = new ChannelList(jsonContent);
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
            }
        }
    }
}
