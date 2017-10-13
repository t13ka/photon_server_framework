using System.Configuration;
using Warsmiths.Server.LoadShedding;

namespace Warsmiths.Server.LoadBalancer.Configuration
{
    internal class LoadBalancerWeight : ConfigurationElement
    {
        [ConfigurationProperty("Level", IsRequired = true)]
        public FeedbackLevel Level
        {
            get { return (FeedbackLevel)this["Level"]; }
            set { this["Level"] = value; }
        }


        [ConfigurationProperty("Value", IsRequired = true)]
        public int Value
        {
            get { return (int)this["Value"]; }
            set { this["Value"] = value; }
        }
    }
}
