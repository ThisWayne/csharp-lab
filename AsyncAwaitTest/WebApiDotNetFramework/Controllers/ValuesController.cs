using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TestClassLibrary;

namespace DotNetFramework.Controllers
{
    public class ValuesController : ApiController
    {
        static HttpClient httpClient = new HttpClient();
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            AsyncAwaitTestClass testClass = new AsyncAwaitTestClass();
            await testClass.TestStart();

            return new string[] { "value1", "value2" };
        }
    }
}
