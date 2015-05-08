using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;
using System.Text;

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

        public ActionResult Authentication()
        {
            var sb = new StringBuilder();

            #region Server 3 Access Step 1
            var RequestString = JsonConvert.SerializeObject(new { Value = ConfigurationManager.AppSettings["KhalafServerID"].EncryptText(ConfigurationManager.AppSettings["KeyMerchantA"]) }, Formatting.None);
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
                Result = JsonConvert.DeserializeObject<dynamic>(response.Content.Replace("\"", null).DecryptText(ConfigurationManager.AppSettings["KeyMerchantA"])).Message2.ToString();
            }
            catch{}
            #endregion Server 3 Access Step 1

            #region Server 3 Access Step 2
            if (!string.IsNullOrWhiteSpace(Result))
            {
                sb.Append("Step 1: MerchantA -> NCDC - <b>introducing</b><br/><br/>");
                sb.AppendFormat("Step 2: NCDC -> MerchantA - <b>{0}</b><br/><br/>", Result);

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
                sb.Append("Step 3: MerchantA -> NCDC - <b>Asking access to MCI</b><br/><br/>");
                sb.AppendFormat("Step 4: NCDC -> MerchantA - <b>{0}</b><br/><br/>", Result);

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
                sb.Append("Step 5: MerchantA -> MCI - <b>Asking for Access Key</b><br/><br/>");
                sb.AppendFormat("Step 6: MCI -> MerchantA - <b>Here is access key {0}, expiration {1}</b><br/><br/>", Result, DateTime.Now.AddMinutes(5));
                Session["ExpirationDate"] = DateTime.Now.AddMinutes(5);
                Session["MCIAccessKey"] = Result;
                ViewBag.Success = true;
                ViewBag.Message = sb.ToString();
            }
            #endregion Execute Test Operation on Server 3

            return View("Index");
        }

        public ActionResult Calculate()
        {
            var Key = (string)Session["MCIAccessKey"];

            var RequestString = JsonConvert.SerializeObject(new { Value = Key }, Formatting.None);
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["MciUrl"] + "do-some-job/");
            var request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddParameter("application/json", RequestString, ParameterType.RequestBody);

            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var Result = response.Content;
                ViewBag.ExpDate = Session["ExpirationDate"];
                ViewBag.CalcSuccess = true;
            }
            else
            {
                ViewBag.NotTrusted = true;
            }

            return View("Index");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return Redirect("~/");
        }
	}
}