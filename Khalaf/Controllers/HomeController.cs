using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using Core;

namespace Khalaf.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> DoJob()
        {

            #region Server 3 Access Step 1
            var RequestString = JsonConvert.SerializeObject(new { Value = ConfigurationManager.AppSettings["KhalafServerID"].Encrypt(ConfigurationManager.AppSettings["KeyKhalaf"]) }, Formatting.None);
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["Server2Url"] + "server3-access-step1/");            
            var request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddParameter("application/json", RequestString, ParameterType.RequestBody);

            // Sending request for calculation and getting response
            var tcs = new TaskCompletionSource<string>();
            client.ExecuteAsync(request, response =>
            {
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var JSON = JsonConvert.DeserializeObject<dynamic>(response.Content.Replace("\"", null).Decrypt(ConfigurationManager.AppSettings["KeyKhalaf"]));
                        tcs.SetResult(JSON.Message2.ToString());
                        
                    }
                    catch
                    {
                        tcs.SetResult(null);
                    }
                }
                else
                {
                    tcs.SetResult(null);
                }
            });
            var Result = await tcs.Task;
            #endregion Server 3 Access Step 1

            #region Server 3 Access Step 2
            if (!string.IsNullOrWhiteSpace(Result))
            {
                RequestString = JsonConvert.SerializeObject(new { Value = Result }, Formatting.None);                
                client = new RestClient();
                client.BaseUrl = new Uri(ConfigurationManager.AppSettings["Server2Url"] + "server3-access-step2/");
                request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json; charset=UTF-8");
                request.AddParameter("application/json", RequestString, ParameterType.RequestBody);


                tcs = new TaskCompletionSource<string>();
                client.ExecuteAsync(request, response =>
                {
                   if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        try
                        {
                            tcs.SetResult(response.Content.Replace("\"", null));
                        
                        }
                        catch
                        {
                            tcs.SetResult(null);
                        }
                    }
                    else
                    {
                        tcs.SetResult(null);
                    }
                });
            }
            Result = await tcs.Task;
            #endregion Server 3 Access Step 2         
            
            #region Access Server 3
            if (!string.IsNullOrWhiteSpace(Result))
            {
                RequestString = JsonConvert.SerializeObject(new { Value = Result }, Formatting.None);                
                client = new RestClient();
                client.BaseUrl = new Uri(ConfigurationManager.AppSettings["Server3Url"] + "get-access/");
                request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json; charset=UTF-8");
                request.AddParameter("application/json", RequestString, ParameterType.RequestBody);

                var response = client.Execute(request);
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
                client.BaseUrl = new Uri(ConfigurationManager.AppSettings["Server3Url"] + "do-some-job/");
                request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json; charset=UTF-8");
                request.AddParameter("application/json", RequestString, ParameterType.RequestBody);

                var response = client.Execute(request);
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