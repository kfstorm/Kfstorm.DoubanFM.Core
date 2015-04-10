using System;

namespace Kfstorm.DoubanFM.Core
{
    public class Song : IEquatable<Song>
    {
        public string AlbumUrl { get; set; }
        public string PictureUrl { get; set; }
        public string Ssid { get; set; }
        public string Artist { get; set; }
        public string Url { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        public double AverageRating { get; set; }
        public int Length { get; set; }
        public string SubType { get; set; }
        public int PublishTime { get; set; }
        public int SongListsCount { get; set; }
        public string Sid { get; }
        public string Aid { get; set; }
        public string Sha256 { get; set; }
        public int Kbps { get; set; }
        public string AlbumTitle { get; set; }
        public bool Like { get; set; }

        public Song(string sid)
        {
            Sid = sid;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Song)obj);
        }

        public override int GetHashCode()
        {
            return Sid.GetHashCode();
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
    }
}