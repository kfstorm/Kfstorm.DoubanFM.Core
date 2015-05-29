using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Represents an error returned by server
    /// </summary>
    public class ServerException : Exception
    {
        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public int Code { get; }
        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerException"/> class.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        public ServerException(int code, string errorMessage) : this(code, errorMessage, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerException"/> class.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServerException(int code, string errorMessage, Exception innerException)
            : base($"code: {code} msg: {errorMessage}", innerException)
        {
            Code = code;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// If the action throws WebException, then parse the exception and rethrow a new instance of <see cref="ServerException"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="Kfstorm.DoubanFM.Core.ServerException"></exception>
        public static async Task<T> TryThrow<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
            {
                var stream = ex.Response.GetResponseStream();
                // ReSharper disable once AssignNullToNotNullAttribute
                var reader = new StreamReader(stream, Encoding.UTF8);
                var jsonContent = await reader.ReadToEndAsync();
                var obj = JObject.Parse(jsonContent);
                var code = (int)obj["code"];
                var message = (string)obj["msg"];
                throw new ServerException(code, message, ex);
            }
        }

        /// <summary>
        /// If the content contains error, then parse the content and throw a new instance of <see cref="ServerException"/>.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <exception cref="Kfstorm.DoubanFM.Core.ServerException"></exception>
        public static void TryThrow(string jsonContent)
        {
            JObject obj;
            try
            {
                obj = JObject.Parse(jsonContent);
            }
            catch
            {
                // Ignore
                return;
            }
            JToken codeToken;
            if (obj.TryGetValue("r", out codeToken))
            {
                var code = (int)codeToken;
                if (code != 0)
                {
                    var message = (string)obj["err"];
                    throw new ServerException(code, message);
                }
            }
        }
    }
}
