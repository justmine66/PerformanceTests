using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTests.Common
{
    public class Helper
    {
        public static async Task EatExceptionAsync(Action action, Action exceptionHandler = null)
        {
            try
            {
                await Task.Run(action);
            }
            catch (Exception)
            {
                // ignored
                exceptionHandler?.Invoke();
            }
        }
        public static async Task<T> EatExceptionAsync<T>(Func<Task<T>> func, Action exceptionHandler = null)
        {
            try
            {
                return await func();
            }
            catch (Exception)
            {
                // ignored
                exceptionHandler?.Invoke();
                return default(T);
            }
        }
    }
}
