using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TwitchBot
{
    internal class ConfigurationReader
    {
		public ConfigurationReader(string configFileName)
		{
            _configFileName = configFileName;
            ReadConfig();
		}

        private const int DEFAULT_MESSAGE_INTERVAL = 120;
        private const int MINIMUM_MESSAGE_INTERVAL = 30;

        private string _configFileName;
        private List<string> _configuredMessages = null;
        private int? _configuredMessageInterval = null;

        public List<string> GetConfiguredMessages()
        {
            return _configuredMessages;
        }

        public int GetConfiguredMessageIntervalInSeconds()
        {
            return _configuredMessageInterval ?? DEFAULT_MESSAGE_INTERVAL;
        }

        private void ReadConfig()
        {
            if (string.IsNullOrEmpty(_configFileName))
            {
                return;
            }

            Stream configStream = new FileStream(_configFileName, FileMode.Open, FileAccess.Read);
            XmlDocument document = new XmlDocument();
            document.Load(configStream);

            XmlNode intervalNode = document.DocumentElement.SelectSingleNode("/config/interval");
            XmlAttributeCollection attributes;
            XmlNode waitTimeNode;
            if (intervalNode != null && (attributes = intervalNode.Attributes) != null && (waitTimeNode = attributes.GetNamedItem("wait-time")) != null)
            {
                string value = waitTimeNode.Value;
                _configuredMessageInterval = int.Parse(value);
                if (_configuredMessageInterval < MINIMUM_MESSAGE_INTERVAL)
                {
                    _configuredMessageInterval = MINIMUM_MESSAGE_INTERVAL;
                }
            }
            else
            {
                _configuredMessageInterval = DEFAULT_MESSAGE_INTERVAL;
            }

            XmlNodeList messagesList = document.DocumentElement.SelectNodes("/config/messages/message");
            if (messagesList != null)
            {
                _configuredMessages = new List<string>();
                foreach( XmlNode messageNode in messagesList)
                {
                    if (messageNode != null && !string.IsNullOrEmpty(messageNode.InnerText))
                    {
                        _configuredMessages.Add(messageNode.InnerText);
                    }
                }
            }
            configStream.Close();
        }
    }
}
