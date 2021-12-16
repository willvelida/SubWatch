using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubWatch.Common.Models
{
    public class Subscription
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SubscriptionType { get; set; }
        public double RenewalCost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime RenewalDate { get; set; }
        public RenewalFrequency RenewalFrequency { get; set; }
        public SubscriptionStatus Status { get; set; }
        public double YearlyCost { get; set; }
    }
}
