using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Server3.Models;
using System.Configuration;
using Core;

namespace Server3.Controllers
{
    public class MainController : ApiController
    {
        [HttpPost]
        [Route("get-access/")]
        public async Task<HttpResponseMessage> GetAccess(SimpleInput Input)
        {
            var Result = await Task.Run(() =>
            {
                // We need try - catch block here to detect whether the decryption of message went successfully
                // Decrypt() method throws an exception if it can't decrypt a message
                try
                {
                    // Decrypting message received from Khalaf Server
                    var DecryptedMessage = Input.Value.Decrypt(ConfigurationManager.AppSettings["KeyServer3"]);

                    // 5 Hour access key
                    return DateTime.Now.AddHours(5).ToString().Encrypt(ConfigurationManager.AppSettings["KeyServer3"]);
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
        [Route("do-some-job/")]
        public async Task<HttpResponseMessage> DoSomeJob(SimpleInput Input)
        {
            var Result = await Task.Run(() =>
            {
                // We need try - catch block here to detect whether the decryption of message went successfully
                // Decrypt() method throws an exception if it can't decrypt a message
                try
                {
                    
                    // Decrypting AccessKey and checking for expiration
                    if (Convert.ToDateTime(Input.Value.Decrypt(ConfigurationManager.AppSettings["KeyServer3"])) > DateTime.Now)
                    {
                        return (int?)(5 + 5);
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
    }
}
