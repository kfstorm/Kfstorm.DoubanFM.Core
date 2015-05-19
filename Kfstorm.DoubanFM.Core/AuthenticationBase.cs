using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// A abstract base class for all implementations of <see cref="IAuthentication" /> interface
    /// </summary>
    public abstract class AuthenticationBase : IAuthentication
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILog Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationBase"/> class.
        /// </summary>
        /// <param name="serverConnection">The server connection.</param>
        protected AuthenticationBase(IServerConnection serverConnection)
        {
            ServerConnection = serverConnection;
            Logger = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// Gets the server connection.
        /// </summary>
        /// <value>
        /// The server connection.
        /// </value>
        protected IServerConnection ServerConnection { get; }

        /// <summary>
        /// Parses the result of logging on.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns>The user information in the result.</returns>
        protected UserInfo ParseLogOnResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            return new UserInfo
            {
                AccessToken = (string)obj[StringTable.AccessToken],
                Username = (string)obj[StringTable.DoubanUsername],
                UserId = (long)obj[StringTable.DoubanUserId],
                ExpiresIn = (long)obj[StringTable.ExpiresIn],
                RefreshToken = (string)obj[StringTable.RefreshToken],
            };
        }

        /// <summary>
        /// Authenticates and returns user info.
        /// </summary>
        /// <returns>
        /// The user info, including username and token.
        /// </returns>
        public abstract Task<UserInfo> Authenticate();
    }
}
