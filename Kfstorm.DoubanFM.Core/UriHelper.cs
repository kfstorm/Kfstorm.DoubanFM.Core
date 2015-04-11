using System;
using System.Collections.Generic;
using System.Linq;

namespace Kfstorm.DoubanFM.Core
{
    public static class UriHelper
    {
        public static void AppendQuery(this UriBuilder uriBuilder, string name, string value)
        {
            name = Uri.EscapeDataString(name);
            value = Uri.EscapeDataString(value);
            var queryToAppend = $"{name}={value}";

            if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
            {
                uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + queryToAppend;
            }
            else
            {
                uriBuilder.Query = queryToAppend;
            }
        }

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
