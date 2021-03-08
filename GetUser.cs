using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using SOH21DryRunFunctionApp.Models;

//sridhar helped in fixing this code
namespace SOH21DryRunFunctionApp
{
    public static class GetUser
    {
        [FunctionName("GetUser")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{id}")] HttpRequest req,
            [CosmosDB("%RatingsDbName%", "%RatingsCollectionName%", ConnectionStringSetting = @"RatingsDatabase",
             SqlQuery = "Select * from users r where r.id = {id}")] IEnumerable<RatingModel> rating,
            ILogger log)
        {
            log.LogInformation("Getting User");
            if (rating == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(rating);
            }
        }
    }
}


