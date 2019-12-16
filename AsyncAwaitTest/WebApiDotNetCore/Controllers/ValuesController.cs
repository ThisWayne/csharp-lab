using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestClassLibrary;

namespace DotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        static HttpClient httpClient = new HttpClient();
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            Debug.WriteLine("===== .Net Core Web API =====");

            AsyncAwaitTestClass testClass = new AsyncAwaitTestClass();
            await testClass.TestStart();
            return new string[] { "value1", "value2" };
        }
    }
}
