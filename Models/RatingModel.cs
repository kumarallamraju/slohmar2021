using System;

namespace SOH21DryRunFunctionApp.Models
{
    public class RatingModel
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public DateTime timestamp { get; set; }
        public string timeStamp { get; internal set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
    }
}