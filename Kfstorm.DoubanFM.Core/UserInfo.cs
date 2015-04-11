using Newtonsoft.Json;

namespace Kfstorm.DoubanFM.Core
{
    public class UserInfo
    {
        public string AccessToken { get; set; }
        public string Username { get; set; }
        public long UserId { get; set; }
        public long ExpiresIn { get; set; }
        public string RefreshToken { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
