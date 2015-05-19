namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Represents a list of channels, including many channel groups.
    /// </summary>
    public class ChannelList
    {
        /// <summary>
        /// Gets the channel groups.
        /// </summary>
        /// <value>
        /// The channel groups.
        /// </value>
        public ChannelGroup[] ChannelGroups { get; internal set; }
    }
}