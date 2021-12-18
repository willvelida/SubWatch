using SubWatch.Common.Models;

namespace SubWatch.Common.Request
{
    public class SubscriptionRequestDto
    {
        public string Name { get; set; }
        public string SubscriptionType { get; set; }
        public double RenewalCost { get; set; }
        public DateTime StartDate { get; set; }
        public RenewalFrequency RenewalFrequency { get; set; }
    }
}
