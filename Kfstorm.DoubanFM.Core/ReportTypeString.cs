using System;
using System.Collections.Generic;

namespace Kfstorm.DoubanFM.Core
{
    internal static class ReportTypeString
    {
        private static readonly Dictionary<ReportType, string> EnumStringMapping = new Dictionary<ReportType, string>
        {
            {ReportType.NewChannel, "n" },
            {ReportType.BanSong, "b" },
            {ReportType.PlayListEmpty, "p" },
            {ReportType.SkipSong, "s" },
            {ReportType.SongEnded, "e" },
            {ReportType.Like, "r" },
            {ReportType.CancelLike, "u" },
        };

        public static string GetString(ReportType type)
        {
            string stringValue;
            if (EnumStringMapping.TryGetValue(type, out stringValue))
            {
                return stringValue;
            }
            throw new ArgumentOutOfRangeException("type");
        }
    }
}