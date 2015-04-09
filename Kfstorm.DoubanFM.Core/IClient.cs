namespace Kfstorm.DoubanFM.Core
{
    public interface IClient
    {
        IPlayer Player { get; }
        ISession Session { get; }
        IServerConnection ServerConnection { get; }

        void Close();
    }
}