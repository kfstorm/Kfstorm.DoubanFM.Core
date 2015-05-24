using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Controls communication with server
    /// </summary>
    public interface IServerConnection
    {
        /// <summary>
        /// Gets the context. The context contains contextual information about server connection, such as client ID and access token.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        IDictionary<string, string> Context { get; }
        /// <summary>
        /// Gets or sets the client ID.
        /// </summary>
        /// <value>
        /// The client ID.
        /// </value>
        string ClientId { get; set; }
        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        string ClientSecret { get; set; }
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        string AppName { get; set; }
        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        string AppVersion { get; set; }
        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        /// <value>
        /// The redirect URI.
        /// </value>
        Uri RedirectUri { get; set; }
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        string AccessToken { get; set; }
        /// <summary>
        /// Gets or sets the UDID.
        /// </summary>
        /// <value>
        /// The UDID.
        /// </value>
        string Udid { get; set; }

        /// <summary>
        /// Sets the session information to request.
        /// </summary>
        /// <param name="request">The request.</param>
        void SetSessionInfoToRequest(HttpWebRequest request);

        /// <summary>
        /// Send an HTTP GET request to the specified URI, and get the response content as string.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="modifier">The modifier to change the request before sending. The modifier can be null.</param>
        /// <returns>The content of response.</returns>
        Task<string> Get(Uri uri, Action<HttpWebRequest> modifier);
        /// <summary>
        /// Send an HTTP POST request to the specified URI, and get the response content as string.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="data">The binary data need to be posted. Null or empty array means no data.</param>
        /// <returns>The content of response.</returns>
        Task<string> Post(Uri uri, byte[] data);
        /// <summary>
        /// Send an HTTP POST request to the specified URI, and get the response content as string.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="data">The binary data need to be posted. Null or empty array means no data.</param>
        /// <param name="modifier">The modifier to change the request before sending. The modifier can be null.</param>
        /// <returns>The content of response.</returns>
        Task<string> Post(Uri uri, byte[] data, Action<HttpWebRequest> modifier);
    }
}