using System;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Represents a song in douban.fm
    /// </summary>
    public class Song : IEquatable<Song>
    {
        /// <summary>
        /// Gets or sets the album URL.
        /// </summary>
        /// <value>
        /// The album URL.
        /// </value>
        public string AlbumUrl { get; set; }
        /// <summary>
        /// Gets or sets the picture URL of cover.
        /// </summary>
        /// <value>
        /// The picture URL of cover.
        /// </value>
        public string PictureUrl { get; set; }
        /// <summary>
        /// Gets or sets the SSID.
        /// </summary>
        /// <value>
        /// The SSID.
        /// </value>
        public string Ssid { get; set; }
        /// <summary>
        /// Gets or sets the artist.
        /// </summary>
        /// <value>
        /// The artist.
        /// </value>
        public string Artist { get; set; }
        /// <summary>
        /// Gets or sets the URL of music file.
        /// </summary>
        /// <value>
        /// The URL of music file.
        /// </value>
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>
        /// The company.
        /// </value>
        public string Company { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the average rating.
        /// </summary>
        /// <value>
        /// The average rating.
        /// </value>
        public double? AverageRating { get; set; }
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length { get; set; }
        /// <summary>
        /// Gets or sets the sub type.
        /// </summary>
        /// <value>
        /// The sub type.
        /// </value>
        public string SubType { get; set; }
        /// <summary>
        /// Gets or sets the publish time.
        /// </summary>
        /// <value>
        /// The publish time.
        /// </value>
        public int? PublishTime { get; set; }
        /// <summary>
        /// Gets or sets the song lists count.
        /// </summary>
        /// <value>
        /// The song lists count.
        /// </value>
        public int? SongListsCount { get; set; }
        /// <summary>
        /// Gets the SID.
        /// </summary>
        /// <value>
        /// The SID.
        /// </value>
        public string Sid { get; }
        /// <summary>
        /// Gets or sets the AID.
        /// </summary>
        /// <value>
        /// The AID.
        /// </value>
        public string Aid { get; set; }
        /// <summary>
        /// Gets or sets the SHA256.
        /// </summary>
        /// <value>
        /// The SHA256.
        /// </value>
        public string Sha256 { get; set; }
        /// <summary>
        /// Gets or sets the bit rate.
        /// </summary>
        /// <value>
        /// The bit rate.
        /// </value>
        public int? Kbps { get; set; }
        /// <summary>
        /// Gets or sets the album title.
        /// </summary>
        /// <value>
        /// The album title.
        /// </value>
        public string AlbumTitle { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Song"/> is marked with red heart.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is marked with red heart; otherwise, <c>false</c>.
        /// </value>
        public bool Like { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Song"/> class.
        /// </summary>
        /// <param name="sid">The SID.</param>
        public Song(string sid)
        {
            Sid = sid;
        }

#pragma warning disable 1591
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Song)obj);
        }

        public override int GetHashCode()
        {
            return Sid?.GetHashCode() ?? 0;
        }

        public bool Equals(Song other)
        {
            return other != null && Sid == other.Sid;
        }

        public static bool operator ==(Song left, Song right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Song left, Song right)
        {
            return !(left == right);
        }

        public override string ToString()
#pragma warning restore 1591
        {
            return $"Title: {Title}, Artist: {Artist}, AlbumTitle: {AlbumTitle}, Sid: {Sid}";
        }
    }
}