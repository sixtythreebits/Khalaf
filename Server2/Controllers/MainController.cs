using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Server2.Models;
using Core;
using System.Configuration;
using Newtonsoft.Json;

namespace Server2.Controllers
{
    public class MainController : ApiController
    {
        [HttpPost]
        [Route("server3-access-step1/")]
        public async Task<HttpResponseMessage> Server3AccessStep1(SimpleInput Input)
        {
            var Result = await Task.Run(() =>
            {
                // We need try - catch block here to detect whether the decryption of message went successfully
                // Decrypt() method throws an exception if it can't decrypt a message
                try
                {
                    //Check if KhalafServer is asking for an access
                    if (Input.Value.Decrypt(ConfigurationManager.AppSettings["KeyKhalaf"]) == ConfigurationManager.AppSettings["KhalafServerID"])
                    {
                        return JsonConvert.SerializeObject(
                        new
                        {
                            Message1 = "Message1",
                            Message2 = "Message2".Encrypt(ConfigurationManager.AppSettings["KeyServer2"])
                        }
                        , Formatting.None).Encrypt(ConfigurationManager.AppSettings["KeyKhalaf"]);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            });

            if (Result == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, Result);
            }
        }

        [HttpPost]
        [Route("server3-access-step2/")]
        public async Task<HttpResponseMessage> Server3AccessStep2(SimpleInput Input)
        {
            var Result = await Task.Run(() =>
            {
                // We need try - catch block here to detect whether the decryption of message went successfully
                // Decrypt() method throws an exception if it can't decrypt a message
                try
                {
                    // Decrypting message received from Khalaf Server
                    var DecryptedMessage = Input.Value.Decrypt(ConfigurationManager.AppSettings["KeyServer2"]);

                    return "Message for Server 3".Encrypt(ConfigurationManager.AppSettings["KeyServer3"]);
                }
                catch
                {
                    return null;
                }
            });

            if (Result == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, Result);
            }
        }
    }
}
