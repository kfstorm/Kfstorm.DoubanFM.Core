using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Indicates a method of user authentication
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// Authenticates and returns user info.
        /// </summary>
        /// <returns>The user info, including username and token.</returns>
        Task<UserInfo> Authenticate();
    }
}
