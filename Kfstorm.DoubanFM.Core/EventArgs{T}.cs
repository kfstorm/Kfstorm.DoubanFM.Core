using System;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// A simple subclass of EventArgs which contains single data object.
    /// </summary>
    /// <typeparam name="T">The type of data object.</typeparam>
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets or sets the data object.
        /// </summary>
        /// <value>
        /// The data object.
        /// </value>
        public T Object { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T}"/> class.
        /// </summary>
        /// <param name="obj">The data object.</param>
        public EventArgs(T obj)
        {
            Object = obj;
        }
    }
}
