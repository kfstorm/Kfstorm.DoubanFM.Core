using System;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// This type of exception will be thrown when some operations which need a selected channel are triggerred but the current channel of the player is not selected.
    /// </summary>
    public class ChannelNotSelectedException : Exception
    {
    }
}
