using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;

namespace MerchantA.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DoJob()
        {

            #region Server 3 Access Step 1
            var RequestString = JsonConvert.SerializeObject(new { Value = ConfigurationManager.AppSettings["KhalafServerID"].EncryptMAC(ConfigurationManager.AppSettings["KeyMerchantA"]) }, Formatting.None);
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["NcdcUrl"] + "server3-access-step1/");
            var request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddParameter("application/json", RequestString, ParameterType.RequestBody);

            var response = client.Execute(request);
            string Result = null;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            try
            {
                Result = JsonConvert.DeserializeObject<dynamic>(response.Content.Replace("\"", null).DecryptMAC(ConfigurationManager.AppSettings["KeyMerchantA"])).Message2.ToString();
            }
            catch{}
            #endregion Server 3 Access Step 1

            #region Server 3 Access Step 2
            if (!string.IsNullOrWhiteSpace(Result))
            {
                RequestString = JsonConvert.SerializeObject(new { Value = Result }, Formatting.None);
                client = new RestClient();
                client.BaseUrl = new Uri(ConfigurationManager.AppSettings["NcdcUrl"] + "server3-access-step2/");
                request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json; charset=UTF-8");
                request.AddParameter("application/json", RequestString, ParameterType.RequestBody);

                response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        Result = response.Content.Replace("\"", null);
                    }
                    catch { }
                }
            }
            #endregion Server 3 Access Step 2

            #region Access Server 3
            if (!string.IsNullOrWhiteSpace(Result))
            {
                RequestString = JsonConvert.SerializeObject(new { Value = Result }, Formatting.None);
                client = new RestClient();
                client.BaseUrl = new Uri(ConfigurationManager.AppSettings["MciUrl"] + "get-access/");
                request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json; charset=UTF-8");
                request.AddParameter("application/json", RequestString, ParameterType.RequestBody);

                response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Result = response.Content.Replace("\"", null);
                }
            }
            #endregion Access Server 3

            #region Execute Test Operation on Server 3
            if (!string.IsNullOrWhiteSpace(Result))
            {
                RequestString = JsonConvert.SerializeObject(new { Value = Result }, Formatting.None);
                client = new RestClient();
                client.BaseUrl = new Uri(ConfigurationManager.AppSettings["MciUrl"] + "do-some-job/");
                request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json; charset=UTF-8");
                request.AddParameter("application/json", RequestString, ParameterType.RequestBody);

                response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Result = response.Content;
                    ViewBag.Success = true;
                }
            }
            #endregion Execute Test Operation on Server 3

            return View("Index");
        }
	}
}