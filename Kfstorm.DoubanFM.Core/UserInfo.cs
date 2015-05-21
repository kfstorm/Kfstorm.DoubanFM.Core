using Newtonsoft.Json;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Represents the user information returned in authentication result
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken { get; set; }
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        /// <value>
        /// The user ID.
        /// </value>
        public long UserId { get; set; }
        /// <summary>
        /// Gets or sets the seconds which the access token will expire in.
        /// </summary>
        /// <value>
        /// The seconds which the access token will expire in.
        /// </value>
        public long ExpiresIn { get; set; }
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string RefreshToken { get; set; }

#pragma warning disable 1591
        public override string ToString()
#pragma warning restore 1591
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
