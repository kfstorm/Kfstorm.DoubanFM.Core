using System;

namespace Kfstorm.DoubanFM.Core
{
    public class EventArgs<T> : EventArgs
    {
        public T Object { get; set; }

        public EventArgs(T obj)
        {
            Object = obj;
        }
    }
}
