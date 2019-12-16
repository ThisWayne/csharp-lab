using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestClassLibrary;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Debug.WriteLine("===== .Net Framework Console App =====");

            AsyncAwaitTestClass testClass = new AsyncAwaitTestClass();
            await testClass.TestStart();

            Console.WriteLine("===== Press enter to stop. =====");
            Console.ReadLine();
        }
    }
}
