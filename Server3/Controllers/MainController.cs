using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Server3.Models;

namespace Server3.Controllers
{
    public class MainController : ApiController
    {
        [HttpPost]
        [Route("save-result/")]
        public async Task<HttpResponseMessage> DoCalculationJob(RecievedResult Result)
        {
            await Task.Run(() =>
            {
                // Store result code goes here
            });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
