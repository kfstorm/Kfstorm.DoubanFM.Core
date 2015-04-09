namespace Kfstorm.DoubanFM.Core
{
    public interface ISession
    {
        SessionState State { get; }

        void LogOn();
        void LogOff();
    }
}