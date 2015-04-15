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
                Assert.Fail($"Expected exception of type {typeof (TException)} not found.");
            }
            catch (TException ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected exception of type {typeof(TException)} not found. Found exception of type {ex.GetType()} instead.");
            }
            return null; // To pass build
        }
    }
}
