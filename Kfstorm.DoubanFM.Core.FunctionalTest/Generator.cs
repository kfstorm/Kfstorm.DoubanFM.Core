using System;
using System.Configuration;

namespace Kfstorm.DoubanFM.Core.FunctionalTest
{
    public static class Generator
    {
        public static string ClientId => ConfigurationManager.AppSettings["ClientId"];
        public static string ClientSecret => ConfigurationManager.AppSettings["ClientSecret"];
        public static Uri RedirectUri => new Uri(ConfigurationManager.AppSettings["RedirectUri"]);
        public static string AppName => ConfigurationManager.AppSettings["AppName"];
        public static string AppVersion => ConfigurationManager.AppSettings["AppVersion"];
        public static string Udid => Guid.NewGuid().ToString("N");
        public static string Username => ConfigurationManager.AppSettings["Username"];
        public static string MailAddress => ConfigurationManager.AppSettings["MailAddress"];
        public static string Password => ConfigurationManager.AppSettings["Password"];

        public static IServerConnection ServerConnection => new ServerConnection(ClientId, ClientSecret, AppName, AppVersion, RedirectUri, Udid);
        public static ISession Session => new Session(ServerConnection);
        public static IPlayer Player => new Player(Session);
        public static IDiscovery Discovery => new Discovery(Session);
    }
}
