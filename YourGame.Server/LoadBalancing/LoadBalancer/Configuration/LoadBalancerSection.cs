using System.Configuration;

namespace YourGame.Server.LoadBalancer.Configuration
{
    internal class LoadBalancerSection : ConfigurationSection
    {
        [ConfigurationProperty("LoadBalancerWeights", IsDefaultCollection = true, IsRequired = true)]
        public LoadBalancerWeightsCollection LoadBalancerWeights
        {
            get
            {
                return (LoadBalancerWeightsCollection)base["LoadBalancerWeights"];
            }
        }

        public void Deserialize(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            this.DeserializeElement(reader, serializeCollectionKey);
        }
    }
}
