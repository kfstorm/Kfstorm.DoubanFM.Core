using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Kfstorm.DoubanFM.Core.FunctionalTest
{
    static class AssertEx
    {
        public static async Task<TException> ThrowsAsync<TException>(Func<Task> code) where TException : Exception
        {
            try
            {
                await code();
            }
            catch (TException ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected exception of type {typeof(TException)} not found. Found exception of type {ex.GetType()} instead.");
            }
            Assert.Fail($"Expected exception of type {typeof(TException)} not found.");
            return null; // To pass build
        }
    }
}
