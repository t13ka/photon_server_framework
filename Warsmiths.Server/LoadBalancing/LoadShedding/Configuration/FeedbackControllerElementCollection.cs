using System.Configuration;

namespace Warsmiths.Server.LoadShedding.Configuration
{
    internal class FeedbackControllerElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FeedbackControllerElement(); 
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FeedbackControllerElement)element).Name; 
        }
    }
}
