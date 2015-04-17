using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Server2.Models;

namespace Server2.Controllers
{
    public class MainController : ApiController
    {
        [HttpPost]
        [Route("calculate/")]
        public async Task<HttpResponseMessage> DoCalculationJob(CalculationJobInput Item)
        {
            int? Result = await Task.Run(() =>
            {
                return Item.a + Item.b;
            });
            return Request.CreateResponse(HttpStatusCode.OK, Result);
        }
    }
}
