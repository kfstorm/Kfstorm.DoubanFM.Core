using System;
using System.Collections.Generic;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Manages the mapping between <see cref="ReportType"/> and single letter string
    /// </summary>
    public static class ReportTypeString
    {
        private static readonly Dictionary<ReportType, string> EnumStringMapping = new Dictionary<ReportType, string>
        {
            {ReportType.CurrentChannelChanged, "n" },
            {ReportType.BanCurrentSong, "b" },
            {ReportType.PlayListEmpty, "p" },
            {ReportType.SkipCurrentSong, "s" },
            {ReportType.CurrentSongEnded, "e" },
            {ReportType.Like, "r" },
            {ReportType.CancelLike, "u" },
        };

        /// <summary>
        /// Gets the string based on report type.
        /// </summary>
        /// <param name="type">The report type.</param>
        /// <returns>The string.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static string GetString(ReportType type)
        {
            string stringValue;
            if (EnumStringMapping.TryGetValue(type, out stringValue))
            {
                return stringValue;
            }
            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}