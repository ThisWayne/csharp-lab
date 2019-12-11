using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestClassLibrary
{
    public class AsyncAwaitTestClass
    {
        static HttpClient httpClient = new HttpClient();
        public async Task TestStart()
        {
            Debug.WriteLine("===== Current Thread Info (in TestStart method) =====");
            PrintInfos();

            Debug.WriteLine("===== Test Start =====");

            Debug.WriteLine("1. === Before MethodWithFinishedTask ===");
            PrintInfos();
            await ReturnFinishedTaskAsync().ConfigureAwait(false);
            Debug.WriteLine("3. === After MethodWithFinishedTask ===");
            PrintInfos();

            Debug.WriteLine("=== Before TestAwaitedMethodInnerThread ===");
            PrintInfos();
            await NestedAwaitMethodAsync();
            Debug.WriteLine("=== After TestAwaitedMethodInnerThread ===");
            PrintInfos();
        }

        private static void PrintInfos()
        {
            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
            ThreadPool.GetAvailableThreads(out int workerThreads, out int completionPortThreads);
            Debug.WriteLine(
                "  -- Worker threads: {0}, Completion port threads: {1}, Total threads: {2}",
                maxWorkerThreads - workerThreads,
                maxCompletionPortThreads - completionPortThreads,
                Process.GetCurrentProcess().Threads.Count
            );
            Debug.WriteLine(
                $"     SynchronizationContext: {SynchronizationContext.Current}\n" +
                $"     ManagedThreadId: {Thread.CurrentThread.ManagedThreadId}\n" +
                $"     IsThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread}"
            );
        }


        private async Task<int> ReturnFinishedTaskAsync()
        {
            Debug.WriteLine($"2.   === Begin MethodWithFinishedTask ===");
            PrintInfos();
            return await Task.FromResult<int>(0);
        }

        private async Task<string> NestedAwaitMethodAsync()
        {
            Debug.WriteLine($"  === Begin TestAwaitedMethodInnerThread ===");
            PrintInfos();

            await MethodAsync();

            Debug.WriteLine($"  === After NestedAwaitMethod ===");
            PrintInfos();

            return await httpClient.GetStringAsync("http://www.yahoo.com/");
        }

        private async Task<string> MethodAsync()
        {
            Debug.WriteLine($"    === Begin NestedAwaitMethod ===");
            PrintInfos();

            return await httpClient.GetStringAsync("http://www.google.com/");
        }
    }
}
