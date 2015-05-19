using System;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// A simple subclass of EventArgs which contains single data object.
    /// </summary>
    /// <typeparam name="T">The type of data object.</typeparam>
    public class EventArgs<T> : EventArgs
    {
        public T Object { get; set; }

        public EventArgs(T obj)
        {
            Object = obj;
        }
    }
}
