using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Controls user session
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Gets the user information.
        /// </summary>
        /// <value>
        /// The user information.
        /// </value>
        UserInfo UserInfo { get; }

        /// <summary>
        /// Gets the server connection.
        /// </summary>
        /// <value>
        /// The server connection.
        /// </value>
        IServerConnection ServerConnection { get; }

        /// <summary>
        /// Logs on with specified authentication method.
        /// </summary>
        /// <param name="authentication">The authentication method.</param>
        /// <returns></returns>
        Task LogOn(IAuthentication authentication);
        /// <summary>
        /// Logs off.
        /// </summary>
        void LogOff();
    }
}