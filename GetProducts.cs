using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SOH21DryRunFunctionApp.Models;
using System.Net.Http;
using System.Threading.Tasks;

//comments 
//save it, stage it, commit & push
 
namespace SOH21DryRunFunctionApp
{
    public static class GetProducts
    {
        [FunctionName("GetProducts")]
       public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,ILogger log)
        {
            HttpClient _http = new HttpClient();
            log.LogInformation("Getting Products");
            var response = await _http.GetAsync("https://serverlessohproduct.trafficmanager.net/api/GetProducts");
 
            if (response == null)
            {
                return new NotFoundResult();
            }
            else
            {
                  var resultContent = await response.Content.ReadAsStringAsync();
                return new OkObjectResult(resultContent);
            }
        }
    }
}