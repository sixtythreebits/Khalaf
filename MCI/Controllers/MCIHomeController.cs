using MCI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core;

namespace MCI.Controllers
{
    public class MCIHomeController : ApiController
    {
        [HttpPost]
        [Route("get-access/")]
        public HttpResponseMessage GetAccess(SimpleInput Input)
        {
            string Result = null;
            // We need try - catch block here to detect whether the decryption of message went successfully
            // Decrypt() method throws an exception if it can't decrypt a message
            try
            {
                // Decrypting message received from Khalaf Server
                var DecryptedMessage = Input.Value.DecryptText(ConfigurationManager.AppSettings["KeyMCI"]);

                // 5 minute access key
                Result = DateTime.Now.AddMinutes(5).ToString().EncryptText(ConfigurationManager.AppSettings["KeyMCI"]);
            }
            catch{ }

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
        public HttpResponseMessage DoSomeJob(SimpleInput Input)
        {
            int? Result = null;
            // We need try - catch block here to detect whether the decryption of message went successfully
            // Decrypt() method throws an exception if it can't decrypt a message
            try
            {

                // Decrypting AccessKey and checking for expiration
                if (Convert.ToDateTime(Input.Value.DecryptText(ConfigurationManager.AppSettings["KeyMCI"])) > DateTime.Now)
                {
                    Result = 5 + 5;
                }
            }
            catch{ }

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
