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

            // 不是一看到await methodAsync()，當前thread就跳回到caller
            // methodAsync裡面的code還是會由當前thread執行
            // 可以看到1.跟ReturnFinishedTaskAsync裡面的2.都是同一個thread ID
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

            // 加不加ConfigureAwait(false)後面執行緒會是誰？main/worker/iocp thread？
            await RunTest2_1();
            await RunTest2_2();

            // 加ConfigureAwait(false)後面一定會是iocp thread？
            await RunTest2_3();

            // 連續兩個await呼叫有加ConfigureAwait(false)
            await RunTest2_4();

            // 如果先呼叫一個await有加ConfigureAwait(false)，然後再呼叫一個await不加ConfigureAwait(false)？
            await RunTest2_5();

            // 裡面的有加，外面的沒加ConfigureAwait(false)
            await RunTest2_6();

            // 外面的有加，裡面的沒加ConfigureAwait(false)
            await RunTest2_7();

            Debug.WriteLine("===== Test 2 End =====");
        }

        private async Task RunTest2_1()
        {
            Debug.WriteLine("===== Test 2.1 =====");

            Debug.WriteLine("1. === Before httpClient.GetStringAsync ===");
            PrintInfos();

            await httpClient.GetStringAsync(url); // 不加ConfigureAwait(false)

            Debug.WriteLine("2. === After httpClient.GetStringAsync  ===");
            PrintInfos(); //1.跟2.是同一個thread ID
        }

        private async Task RunTest2_2()
        {
            Debug.WriteLine("===== Test 2.2 =====");

            Debug.WriteLine("1. === Before httpClient.GetStringAsync ===");
            PrintInfos();

            await httpClient.GetStringAsync(url).ConfigureAwait(false); // 加ConfigureAwait(false)

            Debug.WriteLine("2. === After httpClient.GetStringAsync ConfigureAwait(false) ===");
            PrintInfos(); //1.跟2.會是不同thread ID
        }

        private async Task RunTest2_3()
        {
            Debug.WriteLine("===== Test 2.3 =====");

            Debug.WriteLine("1. === Before ReturnFinishedTaskAsync ===");
            PrintInfos();

            await ReturnFinishedTaskAsync().ConfigureAwait(false); // 加ConfigureAwait(false)

            Debug.WriteLine("3. === After ReturnFinishedTaskAsync ConfigureAwait(false) ===");
            PrintInfos();

            // 一般如果是IO bound的程式有加ConfigureAwait(false)之後的code會由iocp thread執行
            // 但如果await裡面的task很快就完成了
            // 本來要準備走await的機制
            // 去檢查task的狀態的時候發現task已經完成了
            // 就會跳過await機制直接由當前thread繼續執行
        }

        private async Task RunTest2_4()
        {
            Debug.WriteLine("===== Test 2.4 =====");

            Debug.WriteLine("1. === Before httpClient.GetStringAsync ===");
            PrintInfos();

            await httpClient.GetStringAsync(url).ConfigureAwait(false);

            Debug.WriteLine("2. === After httpClient.GetStringAsync ConfigureAwait(false) ===");
            PrintInfos();

            await httpClient.GetStringAsync(url).ConfigureAwait(false);

            Debug.WriteLine("3. === After httpClient.GetStringAsync ConfigureAwait(false) ===");
            PrintInfos();
        }

        private async Task RunTest2_5()
        {
            Debug.WriteLine("===== Test 2.5 =====");

            Debug.WriteLine("1. === Before httpClient.GetStringAsync ===");
            PrintInfos();

            await httpClient.GetStringAsync(url).ConfigureAwait(false); // 加ConfigureAwait(false)

            Debug.WriteLine("2. === After httpClient.GetStringAsync ConfigureAwait(false) ===");
            PrintInfos();

            await httpClient.GetStringAsync(url);

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

            // 因為裡面的沒加ConfigureAwait(false)，所以await後續接手會是main thread
            // 裡面最後執行完的是main thread，外面的有加ConfigureAwait(false)
            // 不交由main thread執行ConfigureAwait(false)的後續，所以worker thread接手
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
