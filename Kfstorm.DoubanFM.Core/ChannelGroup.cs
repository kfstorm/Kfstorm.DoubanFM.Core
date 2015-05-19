namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Represents a group of channels
    /// </summary>
    public class ChannelGroup
    {
        /// <summary>
        /// Gets the ID of the group.
        /// </summary>
        /// <value>
        /// The ID of the group.
        /// </value>
        public int GroupId { get; internal set; }
        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public string GroupName { get; internal set; }
        /// <summary>
        /// Gets the channels.
        /// </summary>
        /// <value>
        /// The channels.
        /// </value>
        public Channel[] Channels { get; internal set; }
    }
}