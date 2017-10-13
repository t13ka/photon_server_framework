using System.Configuration;

namespace Warsmiths.Server.LoadShedding.Configuration
{
    internal class FeedbackLevelElement : ConfigurationElement
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
