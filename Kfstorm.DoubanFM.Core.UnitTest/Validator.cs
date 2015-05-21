using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.UnitTest
{
    public static class Validator
    {
        public static void ValidateChannel(Channel channel)
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
        }

        public static void ValidateSong(Song song)
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
