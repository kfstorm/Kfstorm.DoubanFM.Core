using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// A helper class of <see cref="System.Uri"/>
    /// </summary>
    public static class UriHelper
    {
        /// <summary>
        /// Appends the query to a URI.
        /// </summary>
        /// <param name="uriBuilder">The URI.</param>
        /// <param name="name">The query name.</param>
        /// <param name="value">The query value.</param>
        public static void AppendQuery(this UriBuilder uriBuilder, string name, string value)
        {
            name = Uri.EscapeDataString(name);
            value = Uri.EscapeDataString(value??string.Empty);
            if (value == string.Empty)
            {
                return;
            }
            var queryToAppend = $"{name}={value}";

            if (uriBuilder.Query.Length > 1)
            {
                uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + queryToAppend;
            }
            else
            {
                uriBuilder.Query = queryToAppend;
            }
        }

        /// <summary>
        /// Removes the query from a URI.
        /// </summary>
        /// <param name="uriBuilder">The URI.</param>
        /// <returns>The removed query in binary format (encoded with UTF-8)</returns>
        public static byte[] RemoveQuery(this UriBuilder uriBuilder)
        {
            var query = uriBuilder.Query;
            uriBuilder.Query = string.Empty;
            if (query.Length > 0 && query[0] == '?')
            {
                query = query.Substring(1);
            }
            return Encoding.UTF8.GetBytes(query);
        }

        /// <summary>
        /// Appends the authentication common fields.
        /// </summary>
        /// <param name="uriBuilder">The URI.</param>
        /// <param name="serverConnection">The server connection.</param>
        public static void AppendAuthenticationCommonFields(this UriBuilder uriBuilder, IServerConnection serverConnection)
        {
            AppendQuery(uriBuilder, StringTable.ClientId, serverConnection.ClientId);
            AppendQuery(uriBuilder, StringTable.ClientSecret, serverConnection.ClientSecret);
            AppendQuery(uriBuilder, StringTable.RedirectUri, serverConnection.RedirectUri.AbsoluteUri);
        }

        /// <summary>
        /// Appends the usage common fields.
        /// </summary>
        /// <param name="uriBuilder">The URI.</param>
        /// <param name="serverConnection">The server connection.</param>
        public static void AppendUsageCommonFields(this UriBuilder uriBuilder, IServerConnection serverConnection)
        {
            AppendQuery(uriBuilder, StringTable.ApiKey, serverConnection.ClientId);
            AppendQuery(uriBuilder, StringTable.AppName, serverConnection.AppName);
            AppendQuery(uriBuilder, StringTable.Version, serverConnection.AppVersion);
            AppendQuery(uriBuilder, StringTable.Udid, serverConnection.Udid);
        }

        /// <summary>
        /// Gets the queries.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The queries</returns>
        public static IDictionary<string, string> GetQueries(this Uri uri)
        {
            var queries = uri.Query.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            return (from query in queries
                let part = query.Split('=')
                where part.Length == 2 && part[0].Length > 0
                select part).ToDictionary(part => Uri.UnescapeDataString(part[0]), part => Uri.UnescapeDataString(part[1]));
        }
    }
}
