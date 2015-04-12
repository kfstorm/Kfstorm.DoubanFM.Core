using Newtonsoft.Json;

namespace Kfstorm.DoubanFM.Core
{
    public class LogOnResult
    {
        public UserInfo UserInfo { get; set; }
        
        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
