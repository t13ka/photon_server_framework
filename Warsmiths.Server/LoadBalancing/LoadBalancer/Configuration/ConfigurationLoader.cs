using System;
using System.Xml;

namespace Warsmiths.Server.LoadBalancer.Configuration
{
    internal class ConfigurationLoader
    {
        public static bool TryLoadFromFile(string fileName, out LoadBalancerSection section, out string message)
        {
            section = null;
            message = string.Empty;

            try
            {
                section = LoadFromFile(fileName);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public static LoadBalancerSection LoadFromFile(string fileName)
        {
            using (var fileStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                var xmlReader = XmlReader.Create(fileStream);
                xmlReader.MoveToContent();

                var graphSection = new LoadBalancerSection();
                graphSection.Deserialize(xmlReader, false);

                fileStream.Close();

                return graphSection;
            }
        }
    }
}
