using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// The default implementation of <see cref="ISearcher"/>
    /// </summary>
    public class Searcher : ISearcher
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected ILog Logger = LogManager.GetLogger(typeof(Player));

        /// <summary>
        /// Gets the server connection.
        /// </summary>
        /// <value>
        /// The server connection.
        /// </value>
        public IServerConnection ServerConnection { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Searcher" /> class.
        /// </summary>
        /// <param name="serverConnection">The server connection.</param>
        public Searcher(IServerConnection serverConnection)
        {
            ServerConnection = serverConnection;
        }

        /// <summary>
        /// Creates the search channel URI.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="start">The preferred index of the first channel in the returned channel array.</param>
        /// <param name="size">The max size of returned channel array.</param>
        /// <returns></returns>
        protected virtual Uri CreateSearchChannelUri(string query, int start, int size)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/search/channel");
            uriBuilder.AppendUsageCommonFields(ServerConnection);
            uriBuilder.AppendQuery(StringTable.Query, query);
            uriBuilder.AppendQuery(StringTable.Start, start.ToString(CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.Limit, size.ToString(CultureInfo.InvariantCulture));
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Searches the channel with specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="start">The preferred index of the first channel in the returned channel array.</param>
        /// <param name="size">The max size of returned channel array.</param>
        /// <returns>A channel array with the first channel at index <paramref name="start"/>, or an empty array if no channels available.</returns>
        public async Task<Channel[]> SearchChannel(string query, int start, int size)
        {
            var uri = CreateSearchChannelUri(query, start, size);
            var jsonContent = await ServerConnection.Get(uri, null);
            var channelArray = ParseSearchChannelResult(jsonContent);
            Logger.Info($"Got channel search result. Channel count: {channelArray.Length}. Detail: {JsonConvert.SerializeObject(channelArray)}");
            return channelArray;
        }

        /// <summary>
        /// Parses the search channel result.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns>The search channel result.</returns>
        protected virtual Channel[] ParseSearchChannelResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            return obj["channels"].GetArrayOrEmpty().Select(chl => chl.ParseChannel()).ToArray();
        }
    }
}
