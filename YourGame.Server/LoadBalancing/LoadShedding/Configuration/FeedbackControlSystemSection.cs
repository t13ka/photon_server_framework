using System.Configuration;

namespace YourGame.Server.LoadShedding.Configuration
{
    internal class FeedbackControlSystemSection : ConfigurationSection
    {
        [ConfigurationProperty("FeedbackControllers", IsDefaultCollection = true, IsRequired = true)]
        public FeedbackControllerElementCollection FeedbackControllers
        {
            get
            {
                return (FeedbackControllerElementCollection)base["FeedbackControllers"];
            }
        }

        public void Deserialize(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            this.DeserializeElement(reader, serializeCollectionKey);
        }
    }
}
