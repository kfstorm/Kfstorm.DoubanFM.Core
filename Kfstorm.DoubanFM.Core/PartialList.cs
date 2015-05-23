using System.Collections.Generic;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Contains a partial list of a complete list and the total count of the complete list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PartialList<T>
    {
        /// <summary>
        /// Gets or sets the current list.
        /// </summary>
        /// <value>
        /// The current list.
        /// </value>
        public IList<T> CurrentList { get; set; }
        /// <summary>
        /// Gets or sets the total count of the complete list.
        /// </summary>
        /// <value>
        /// The total count of the complete list.
        /// </value>
        public int? TotalCount { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialList{T}"/> class.
        /// </summary>
        public PartialList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialList{T}"/> class.
        /// </summary>
        /// <param name="currentList">The current list.</param>
        public PartialList(IList<T> currentList)
        {
            CurrentList = currentList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialList{T}"/> class.
        /// </summary>
        /// <param name="currentList">The current list.</param>
        /// <param name="totalCount">The total count of the complete list.</param>
        public PartialList(IList<T> currentList, int totalCount)
        {
            CurrentList = currentList;
            TotalCount = totalCount;
        }
    }
}
