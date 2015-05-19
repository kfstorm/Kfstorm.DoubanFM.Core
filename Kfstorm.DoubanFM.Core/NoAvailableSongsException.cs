using System;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// This type of exception will be throwed when trying to get a list of songs for current channel but server returned an empty list.
    /// </summary>
    public class NoAvailableSongsException : Exception
    {
    }
}
