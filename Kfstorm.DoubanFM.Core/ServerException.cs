using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    public class ServerException : Exception
    {
        public int Code { get; }
        public string ErrorMessage { get; }

        public ServerException(int code, string errorMessage) : this(code, errorMessage, null)
        {
        }

        public ServerException(int code, string errorMessage, Exception innerException)
            : base($"code: {code} msg: {errorMessage}", innerException)
        {
            Code = code;
            ErrorMessage = errorMessage;
        }

        public static async Task<T> ThrowIfIsServerException<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                var ex2 = ex as WebException;
                if (ex2 == null) throw;
                var stream = ex2.Response.GetResponseStream();
                var reader = new StreamReader(stream, Encoding.UTF8);
                var jsonContent = await reader.ReadToEndAsync();
                int code;
                string message;
                ParseServerException(jsonContent, out code, out message);
                throw new ServerException(code, message, ex);
            }
        }

        private static void ParseServerException(string jsonContent, out int code, out string message)
        {
            var obj = JObject.Parse(jsonContent);
            code = (int)obj["code"];
            message = (string)obj["msg"];
        }
    }
}
