namespace Kfstorm.DoubanFM.Core
{
    public class ChannelGroup
    {
        public int GroupId { get; internal set; }
        public string GroupName { get; internal set; }
        public Channel[] Channels { get; internal set; }
    }
}