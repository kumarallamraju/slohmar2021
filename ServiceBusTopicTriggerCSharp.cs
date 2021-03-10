using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SOH21DryRunFunctionApp
{
     public class ReceiptData
     {
          public int ItemCount { get; set; }
          public double TotalCost { get; set; }
          public string SalesNumber { get; set; }
          public DateTime SalesDateTime { get; set; }
          public string LocationId { get; set; }
          public string ReceiptImageBase64 { get; set; }
     }
     public static class ServiceBusTopicTriggerCSharp
     {
          [FunctionName("ProcessAllMessages")]
          public static async Task ProcessAllMessages([ServiceBusTrigger("%ServiceBusTopicName%", "%ServiceBusSubscriptionName%", Connection = "ServiceBusConnectionString")]string mySbMsg,
                                            [Blob("receipts/all/{rand-guid}.json", FileAccess.Write, Connection = "DestinationStorageConnectionString")] CloudBlockBlob output,
                                            ILogger log)
          {
               log.LogInformation($"ProcessAllMessagesFunction - ServiceBus topic trigger function processed message: {mySbMsg}");

               ReceiptData receiptInfo = BuildReceipt(mySbMsg);

               // Save receipt JSON to blob
               await output.UploadTextAsync(JsonConvert.SerializeObject(receiptInfo));
          }

          [FunctionName("ProcessFilteredMessages")]
          public static async Task ProcessFilteredMessages([ServiceBusTrigger("%ServiceBusTopicName%", "%ServiceBusFilteredSubscriptionName%", Connection = "ServiceBusConnectionString")]string mySbMsg,
                                            [Blob("receipts/high-value/{rand-guid}.json", FileAccess.Write, Connection = "DestinationStorageConnectionString")] CloudBlockBlob output,
                                            ILogger log)
          {
               log.LogInformation($"ProcessFilteredMessagesFunction - ServiceBus topic trigger function processed message: {mySbMsg}");

               ReceiptData receiptInfo = BuildReceipt(mySbMsg);

               dynamic receipt = JObject.Parse(mySbMsg);
               string receiptUrl = (string)receipt["ReceiptUrl"];
               byte[] receiptBytes = await GetReceiptFromUrl(receiptUrl);

               receiptInfo.ReceiptImageBase64 = Convert.ToBase64String(receiptBytes);

               // Save receipt JSON to blob
               await output.UploadTextAsync(JsonConvert.SerializeObject(receiptInfo));
          }

          private static ReceiptData BuildReceipt(string message)
          {
               dynamic receipt = JObject.Parse(message);

               ReceiptData receiptInfo = new ReceiptData()
               {
                    ItemCount = (int)receipt["ItemCount"],
                    LocationId = (string)receipt["LocationId"],
                    SalesDateTime = DateTime.Parse((string)receipt["SalesDateTime"]),
                    SalesNumber = (string)receipt["SalesNumber"],
                    TotalCost = (double)receipt["TotalCost"]
               };

               return receiptInfo;
          }
          private static async Task<byte[]> GetReceiptFromUrl(string url)
          {
               byte[] results = null;

               using (var webClient = new WebClient())
               {
                    results = await webClient.DownloadDataTaskAsync(url);
               }

               return results;
          }
     }
}