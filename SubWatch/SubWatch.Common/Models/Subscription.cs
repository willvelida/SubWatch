namespace SubWatch.Common.Models
{
    public class Subscription
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SubscriptionType { get; set; }
        public double RenewalCost { get; set; }
        public string StartDate { get; set; }
        public DateTime RenewalDate { get; set; }
        public RenewalFrequency RenewalFrequency { get; set; }
        public double TotalCost { get; set; }
    }
}
