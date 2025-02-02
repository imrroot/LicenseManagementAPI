using LicenseManagementLibrary.src.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManagementLibrary.src.Models
{
    public class Subscription
    {
        public string Name { get; }
        public int AccessLevel { get; }
        public Subscription(CustomerSubscriptionDTO subscription)
        {
            Name = subscription.Name;
            AccessLevel = subscription.AccessLevel;
        }
    }
}
