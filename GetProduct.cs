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
    public static class GetProduct
    {
        [FunctionName("GetProduct")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id}")] HttpRequest req,
            [CosmosDB("%RatingsDbName%", "products", ConnectionStringSetting = @"RatingsDatabase",
             SqlQuery = "Select * from products r where r.id = {id}")] IEnumerable<RatingModel> rating,
            ILogger log)
        {
            log.LogInformation("Getting Product");
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


