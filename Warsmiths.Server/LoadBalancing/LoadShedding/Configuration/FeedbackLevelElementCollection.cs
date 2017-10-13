using System.Configuration;

namespace Warsmiths.Server.LoadShedding.Configuration
{
    internal class FeedbackLevelElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FeedbackLevelElement(); 
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FeedbackLevelElement)element).Level;
        }
    }
}
