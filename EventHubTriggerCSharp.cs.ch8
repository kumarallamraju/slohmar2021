using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.EventHubs;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SOH21DryRunFunctionApp
{
    public class ReceiptInfo
    {
        public int ItemCount { get; set; }
        public double TotalCost { get; set; }
        public string SalesNumber { get; set; }
        public string SalesDateTime { get; set; }
        public string LocationId { get; set; }
        public string ReceiptUrl { get; set; }
    }
    public static class EventHubTriggerCSharp
    {
        [FunctionName("ProcessPointOfSaleEvents")]
        public static async Task Run([EventHubTrigger("%EventHubName%", Connection = "EventHubConnectionString")] EventData[] events,
                                     [CosmosDB(databaseName: "%RatingsDbName%", collectionName: "%CosmosDBCollectionName%", ConnectionStringSetting = "RatingsDatabase")] IAsyncCollector<string> docs,
                                     [ServiceBus(queueOrTopicName: "%ServiceBusTopicName%", EntityType = EntityType.Topic, Connection = "ServiceBusConnectionString")] IAsyncCollector<Message> msgs,
                                     ILogger log)
        {
            var exceptions = new List<Exception>();

            log.LogDebug($"Processing a batch of [{events.Length}] events.");

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");

                    // TODO: Add logic here per instructions in challenge, and then add data to Comsos DB.
                    // Add to CosmosDB
                    await docs.AddAsync(messageBody);

                    // If there is an image of the receipt, send a message to Service Bus.
                    dynamic eventMsg = JObject.Parse(messageBody);

                    if (eventMsg["header"]["receiptUrl"] != null)
                    {
                        log.LogDebug("Found a receipt! Will attempt to add to a Service Bus topic.");

                        // Send to Service Bus
                        ReceiptInfo receipt = new ReceiptInfo
                        {
                            ItemCount = eventMsg["details"].Count,
                            TotalCost = eventMsg["header"]["totalCost"],
                            LocationId = eventMsg["header"]["locationId"],
                            SalesDateTime = eventMsg["header"]["dateTime"],
                            SalesNumber = eventMsg["header"]["salesNumber"],
                            ReceiptUrl = eventMsg["header"]["receiptUrl"],
                        };

                        Message item = new Message
                        {
                            Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(receipt))
                        };
                        item.UserProperties.Add("TotalCost", receipt.TotalCost);

                        await msgs.AddAsync(item);
                    }
                    else
                    {
                        log.LogDebug("No receipt . . . moving on.");
                    }
                }
                catch (Exception e)
                {
                    log.LogError($"Error while processing events: [{e}]");

                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
            {
                var ex = new AggregateException(exceptions);
                log.LogError($"Failed to process multiple messages! {ex}");
                //throw new AggregateException(exceptions);
            }

            if (exceptions.Count == 1)
            {
                log.LogError($"Failed to process messages. {exceptions}");
                //throw exceptions.Single();
            }
        }
    }
}
