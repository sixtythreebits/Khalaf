using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;

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

        public async Task<ActionResult> CalculationJob()
        {
            // Preparing data for calculation
            var Json = new 
            {
                a = 1,
                b = 3
            };

            // Preparing calculation request
            var JsonString = JsonConvert.SerializeObject(Json, Formatting.None);
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["Server2Url"] + "calculate/");            
            var request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddParameter("application/json", JsonString, ParameterType.RequestBody);

            // Sending request for calculation and getting response
            var tcs = new TaskCompletionSource<int?>();
            client.ExecuteAsync(request, response =>
            {
                var StatusCode = response.StatusCode;
                var Content = response.Content;

                int TaskResult;
                if (int.TryParse(response.Content, out TaskResult))
                {
                    tcs.SetResult((int?)TaskResult);
                }
                else
                {
                    tcs.SetResult(null);
                }
                
            });
            var Result = await tcs.Task;


            //if Result is good than we store it
            if (Result > 0)
            {
                // Preparing request parameter
                var Json2 = new
                {
                    result = Result
                };

                // Preparing request
                client = new RestClient();
                client.BaseUrl = new Uri(ConfigurationManager.AppSettings["Server3Url"] + "save-result/");
                request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json; charset=UTF-8");
                request.AddParameter("application/json", JsonString, ParameterType.RequestBody);

                // Sending request to store data and getting response.

                var tcs1 = new TaskCompletionSource<bool>();
                client.ExecuteAsync(request, response =>
                {
                    tcs1.SetResult(response.StatusCode== System.Net.HttpStatusCode.OK);
                });
                var IsSuccess = await tcs1.Task;
                ViewBag.Success = IsSuccess;
            }


            return View("Index");
        }
	}
}