using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.FunctionalTest
{
    [SetUpFixture]
    public class Setup
    {
        [SetUp]
        public void Initialize()
        {
            Generator.DefaultPlayer.Initialize().Wait();
        } 
    }
}
