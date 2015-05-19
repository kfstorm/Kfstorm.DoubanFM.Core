using System;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// This type of exception will be thrown when some operations which need current song is not null are triggerred but the current song of the player is null.
    /// </summary>
    public class SongNotSelectedException : Exception
    {
    }
}