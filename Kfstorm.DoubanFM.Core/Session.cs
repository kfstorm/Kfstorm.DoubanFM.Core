using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// The default implementation of <see cref="ISession"/>
    /// </summary>
    public class Session : ISession
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected ILog Logger = LogManager.GetLogger(typeof(Session));

        /// <summary>
        /// Gets the user information.
        /// </summary>
        /// <value>
        /// The user information.
        /// </value>
        public UserInfo UserInfo { get; protected set; }

        /// <summary>
        /// Gets the server connection.
        /// </summary>
        /// <value>
        /// The server connection.
        /// </value>
        public IServerConnection ServerConnection { get; }

        /// <summary>
        /// Indicates whether the instance in working state.
        /// </summary>
        protected int IsWorking;

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="serverConnection">The server connection.</param>
        public Session(IServerConnection serverConnection)
        {
            ServerConnection = serverConnection;
        }

        /// <summary>
        /// Logs on with specified authentication method.
        /// </summary>
        /// <param name="authentication">The authentication method.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// Already logged on
        /// or
        /// Another unfinished log on/off request exists.
        /// </exception>
        /// <exception cref="System.Exception">Null user info is not allowed</exception>
        public async Task LogOn(IAuthentication authentication)
        {

            if (UserInfo != null)
            {
                throw new InvalidOperationException("Already logged on");
            }
            if (Interlocked.CompareExchange(ref IsWorking, 1, 0) == 0)
            {
                try
                {
                    var result = await authentication.Authenticate();
                    Logger.Info($"Authentication result: {result}");
                    if (result == null)
                    {
                        throw new Exception("Null user info is not allowed");
                    }
                    UserInfo = result;
                    ServerConnection.AccessToken = UserInfo.AccessToken;
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception occurred when trying to log on.", ex);
                    throw;
                }
                finally
                {
                    Interlocked.CompareExchange(ref IsWorking, 0, 1);
                }
            }
            throw new InvalidOperationException("Another unfinished log on/off request exists.");
        }

        /// <summary>
        /// Logs off.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Already logged off
        /// or
        /// Another unfinished log on/off request exists.
        /// </exception>
        public void LogOff()
        {
            if (UserInfo == null)
            {
                throw new InvalidOperationException("Already logged off");
            }
            if (Interlocked.CompareExchange(ref IsWorking, 1, 0) == 0)
            {
                UserInfo = null;
                Interlocked.CompareExchange(ref IsWorking, 0, 1);
                return;
            }
            throw new InvalidOperationException("Another unfinished log on/off request exists.");
        }
    }
}
