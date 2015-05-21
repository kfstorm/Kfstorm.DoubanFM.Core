using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Controls playing logic, including channel/song switch and red heart marking.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        ISession Session { get; }
        /// <summary>
        /// Gets the server connection.
        /// </summary>
        /// <value>
        /// The server connection.
        /// </value>
        IServerConnection ServerConnection { get; }
        /// <summary>
        /// Gets the current song.
        /// </summary>
        /// <value>
        /// The current song.
        /// </value>
        Song CurrentSong { get; }
        /// <summary>
        /// Gets the channel list.
        /// </summary>
        /// <value>
        /// The channel list.
        /// </value>
        /// <remarks>
        /// The channel list is a collection of sample channels, organized by groups.
        /// </remarks>
        ChannelList ChannelList { get; }
        /// <summary>
        /// Gets the current channel.
        /// </summary>
        /// <value>
        /// The current channel.
        /// </value>
        Channel CurrentChannel { get; }
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        /// <remarks>
        /// The configuration stores information needed by player. Such as music format and bit rate, which will be used when communiating with server.
        /// </remarks>
        IDictionary<string, object> Config { get; }

        /// <summary>
        /// Occurs when current song changed.
        /// </summary>
        event EventHandler<EventArgs<Song>> CurrentSongChanged;
        /// <summary>
        /// Occurs when current channel changed.
        /// </summary>
        event EventHandler<EventArgs<Channel>> CurrentChannelChanged;

        /// <summary>
        /// Refreshes the channel list.
        /// </summary>
        /// <returns></returns>
        Task RefreshChannelList();
        /// <summary>
        /// Changes the channel.
        /// </summary>
        /// <param name="newChannel">The new channel.</param>
        /// <param name="type">The type of operation.</param>
        /// <returns></returns>
        Task ChangeChannel(Channel newChannel, ChangeChannelCommandType type = ChangeChannelCommandType.Normal);
        /// <summary>
        /// Switch to next song.
        /// </summary>
        /// <param name="type">the type of next operation.</param>
        /// <returns></returns>
        Task Next(NextCommandType type);
        /// <summary>
        /// Sets the red heart.
        /// </summary>
        /// <param name="redHeart">if set to <c>true</c> then current song will be marked with red heart, indicating that user likes this song. Otherwise remove the red heart mark.</param>
        /// <returns></returns>
        /// <remarks>Set redHeart to false doesn't means user dislike current song.</remarks>
        Task SetRedHeart(bool redHeart);
    }
}