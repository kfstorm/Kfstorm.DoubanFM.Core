using System;

namespace Kfstorm.DoubanFM.Core
{
    public class Channel : IEquatable<Channel>
    {
        public Channel(int id)
        {
            Id = id;
        }

        public bool Equals(Channel other)
        {
            return other != null && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Id { get; }

        public int SongCount { get; set; }

        public string CoverUrl { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Channel)obj);
        }

        public static bool operator ==(Channel left, Channel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Channel left, Channel right)
        {
            return !(left == right);
        }
    }
}