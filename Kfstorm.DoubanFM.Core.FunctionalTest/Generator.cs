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
        public static IServerConnection ServerConnection => new ServerConnection(ClientId, ClientSecret, AppName, AppVersion, RedirectUri);
        public static ISession Session => new Session(ServerConnection);
        public static IPlayer Player => new Player(Session);

        /// <summary>
        /// Return single instance of default player.
        /// </summary>
        public static IPlayer DefaultPlayer { get; } = Player;
    }
}
