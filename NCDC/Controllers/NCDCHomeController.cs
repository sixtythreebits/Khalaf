using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NCDC.Models;
using System.Configuration;
using Core;

namespace NCDC.Controllers
{
    public class NCDCHomeController : ApiController
    {
        [HttpPost]
        [Route("server3-access-step1/")]
        public HttpResponseMessage Server3AccessStep1(SimpleInput Input)
        {
            // We need try - catch block here to detect whether the decryption of message went successfully
            // Decrypt() method throws an exception if it can't decrypt a message
            string Result = null;
            try
            {
                //Check if KhalafServer is asking for an access
                if (Input.Value.DecryptMAC(ConfigurationManager.AppSettings["KeyMerchantA"]) == ConfigurationManager.AppSettings["MerchantAServerID"])
                {
                    Result = JsonConvert.SerializeObject(
                    new
                    {
                        Message1 = "Message1",
                        Message2 = "Message2".EncryptMAC(ConfigurationManager.AppSettings["KeyNCDC"])
                    }
                    , Formatting.None).EncryptMAC(ConfigurationManager.AppSettings["KeyMerchantA"]);
                }
            }
            catch { }

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
        public HttpResponseMessage Server3AccessStep2(SimpleInput Input)
        {
            string Result = null;
            // We need try - catch block here to detect whether the decryption of message went successfully
            // Decrypt() method throws an exception if it can't decrypt a message
            try
            {
                // Decrypting message received from Khalaf Server
                var DecryptedMessage = Input.Value.DecryptMAC(ConfigurationManager.AppSettings["KeyNCDC"]);

                Result = "Message for Server 3".EncryptMAC(ConfigurationManager.AppSettings["KeyMCI"]);
            }
            catch { }

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
