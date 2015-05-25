using System;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Password authentication method
    /// </summary>
    public class PasswordAuthentication : AuthenticationBase
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Authenticates and returns user info.
        /// </summary>
        /// <returns>
        /// The user info, including username and token.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Username is empty
        /// or
        /// Password is empty
        /// </exception>
        public override async Task<UserInfo> Authenticate()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                throw new InvalidOperationException("Username is empty");
            }
            if (string.IsNullOrEmpty(Password))
            {
                throw new InvalidOperationException("Password is empty");
            }

            var uriBuilder = new UriBuilder("https://www.douban.com/service/auth2/token");
            uriBuilder.AppendAuthenticationCommonFields(ServerConnection);
            uriBuilder.AppendQuery(StringTable.GrantType, StringTable.Password);
            uriBuilder.AppendQuery(StringTable.Username, Username);
            uriBuilder.AppendQuery(StringTable.Password, Password);
            var data = uriBuilder.RemoveQuery();
            var jsonContent = await ServerConnection.Post(uriBuilder.Uri, data);
            return ParseLogOnResult(jsonContent);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordAuthentication"/> class.
        /// </summary>
        /// <param name="serverConnection">The server connection.</param>
        public PasswordAuthentication(IServerConnection serverConnection) : base(serverConnection)
        {
        }
    }
}
