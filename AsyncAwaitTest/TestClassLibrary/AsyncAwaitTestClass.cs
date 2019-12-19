using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TestClassLibrary
{
    public class AsyncAwaitTestClass
    {
        static HttpClient httpClient = new HttpClient();
        private string url = "http://www.yahoo.com/";
        public AsyncAwaitTestClass() {
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
        }

        public async Task TestStart()
        {
            Debug.WriteLine("===== Current Thread Info (in TestStart method) =====");
            PrintInfos();

            Debug.WriteLine("===== Test Start =====");

            await RunTest1();
            await RunTest2();
            await RunTest3();

            Debug.WriteLine("===== Test End =====");
        }

        private void PrintInfos()
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

        private async Task RunTest1()
        {
            Debug.WriteLine("===== Test 1 =====");
            
            Debug.WriteLine("1. === Before ReturnFinishedTaskAsync ===");
            PrintInfos();
            await ReturnFinishedTaskAsync();

            Debug.WriteLine("===== Test 1 End =====");
        }

        private async Task<int> ReturnFinishedTaskAsync()
        {
            Debug.WriteLine("2.   === Begin MethodWithFinishedTask ===");
            PrintInfos();
            return await Task.FromResult<int>(0);
        }

        private async Task RunTest2()
        {
            Debug.WriteLine("===== Test 2 =====");

            await RunTest2_1();
            await RunTest2_2();
            await RunTest2_3();
            await RunTest2_4();
            await RunTest2_5();
            await RunTest2_6();
            await RunTest2_7();

            Debug.WriteLine("===== Test 2 End =====");
        }

        private async Task RunTest2_1()
        {
            Debug.WriteLine("===== Test 2.1 =====");

            Debug.WriteLine("1. === Before httpClient.GetStringAsync ===");
            PrintInfos();

            await httpClient.GetStringAsync(url); // without ConfigureAwait(false)

            Debug.WriteLine("2. === After httpClient.GetStringAsync  ===");
            PrintInfos();
        }

        private async Task RunTest2_2()
        {
            Debug.WriteLine("===== Test 2.2 =====");

            Debug.WriteLine("1. === Before httpClient.GetStringAsync ===");
            PrintInfos();

            await httpClient.GetStringAsync(url).ConfigureAwait(false); // with ConfigureAwait(false)

            Debug.WriteLine("2. === After httpClient.GetStringAsync ConfigureAwait(false) ===");
            PrintInfos(); // 1. and 2. would be different thread ID
        }

        private async Task RunTest2_3()
        {
            Debug.WriteLine("===== Test 2.3 =====");

            Debug.WriteLine("1. === Before ReturnFinishedTaskAsync ===");
            PrintInfos();

            await ReturnFinishedTaskAsync().ConfigureAwait(false); // with ConfigureAwait(false)

            Debug.WriteLine("3. === After ReturnFinishedTaskAsync ConfigureAwait(false) ===");
            PrintInfos();
        }

        private async Task RunTest2_4()
        {
            Debug.WriteLine("===== Test 2.4 =====");

            Debug.WriteLine("1. === Before httpClient.GetStringAsync ===");
            PrintInfos();

            await httpClient.GetStringAsync(url).ConfigureAwait(false); // with ConfigureAwait(false)

            Debug.WriteLine("2. === After httpClient.GetStringAsync ConfigureAwait(false) ===");
            PrintInfos();

            await httpClient.GetStringAsync(url).ConfigureAwait(false); // with ConfigureAwait(false)

            Debug.WriteLine("3. === After httpClient.GetStringAsync ConfigureAwait(false) ===");
            PrintInfos();
        }

        private async Task RunTest2_5()
        {
            Debug.WriteLine("===== Test 2.5 =====");

            Debug.WriteLine("1. === Before httpClient.GetStringAsync ===");
            PrintInfos();

            await httpClient.GetStringAsync(url).ConfigureAwait(false); // with ConfigureAwait(false)

            Debug.WriteLine("2. === After httpClient.GetStringAsync ConfigureAwait(false) ===");
            PrintInfos();

            await httpClient.GetStringAsync(url); // without ConfigureAwait(false)

            Debug.WriteLine("3. === After httpClient.GetStringAsync ===");
            PrintInfos();
        }

        private async Task RunTest2_6()
        {
            Debug.WriteLine("===== Test 2.6 =====");

            Debug.WriteLine("1. === Before MethodWithConfigureAwaitFalseInsideAsync ===");
            PrintInfos();

            await MethodWithConfigureAwaitFalseInsideAsync();

            Debug.WriteLine("4. === After MethodWithConfigureAwaitFalseInsideAsync ===");
            PrintInfos();
        }

        private async Task<string> MethodWithConfigureAwaitFalseInsideAsync()
        {
            Debug.WriteLine("2. === Before httpClient.GetStringAsync ConfigureAwait(false) ===");
            PrintInfos();

            var result = await httpClient.GetStringAsync(url).ConfigureAwait(false);

            Debug.WriteLine("3. === After httpClient.GetStringAsync ConfigureAwait(false) ===");
            PrintInfos();

            return result;
        }

        private async Task RunTest2_7()
        {
            Debug.WriteLine("===== Test 2.7 =====");

            Debug.WriteLine("1. === Before MethodWithoutConfigureAwaitFalseInsideAsync ConfigureAwait(false) ===");
            PrintInfos();

            await MethodWithoutConfigureAwaitFalseInsideAsync().ConfigureAwait(false);

            Debug.WriteLine("4. === After MethodWithoutConfigureAwaitFalseInsideAsync ConfigureAwait(false) ===");
            PrintInfos();
        }

        private async Task<string> MethodWithoutConfigureAwaitFalseInsideAsync()
        {
            Debug.WriteLine("2. === Before httpClient.GetStringAsync ===");
            PrintInfos();

            var result = await httpClient.GetStringAsync(url);

            Debug.WriteLine("3. === After httpClient.GetStringAsync ===");
            PrintInfos();

            return result;
        }

        private async Task RunTest3()
        {
            Debug.WriteLine("===== Test 3 =====");

            await RunTest3_1();
            await RunTest3_2();

            Debug.WriteLine("===== Test 3 End =====");
        }

        private async Task RunTest3_1()
        {
            Debug.WriteLine("===== Test 3.1 =====");

            Debug.WriteLine("1. === Before Task.Delay ===");
            PrintInfos();

            await Task.Delay(1000);

            Debug.WriteLine("2. === After Task.Delay ===");
            PrintInfos();
        }

        private async Task RunTest3_2()
        {
            Debug.WriteLine("===== Test 3.2 =====");

            Debug.WriteLine("1. === Before Task.Delay ConfigureAwait(false) ===");
            PrintInfos();

            await Task.Delay(1000).ConfigureAwait(false);

            Debug.WriteLine("2. === After Task.Delay ConfigureAwait(false) ===");
            PrintInfos();
        }
    }
}
