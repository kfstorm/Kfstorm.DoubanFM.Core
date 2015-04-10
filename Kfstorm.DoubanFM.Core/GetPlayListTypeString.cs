using System;

namespace Kfstorm.DoubanFM.Core
{
    internal static class GetPlayListTypeString
    {
        public static string GetString(GetPlayListType type)
        {
            switch (type)
            {
                case GetPlayListType.New:
                    return "n";
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}