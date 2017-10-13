using System.Configuration;

namespace Warsmiths.Server.LoadBalancer.Configuration
{
    internal class LoadBalancerWeightsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoadBalancerWeight(); 
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoadBalancerWeight)element).Level; 
        }
    }
}
