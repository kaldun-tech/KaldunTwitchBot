using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TwitchBot
{
    internal class ConfigurationReader
    {
		public ConfigurationReader(Stream stream)
		{
			_configStream = stream;
		}

        private const int DEFAULT_MESSAGE_INTERVAL = 120;
        private const int MINIMUM_MESSAGE_INTERVAL = 30;

        private Stream _configStream = null;
        private List<string> _configuredMessages = null;
        private int? _configuredMessageInterval = null;

        public List<string> GetConfiguredMessages()
        {
            if (_configuredMessages == null)
            {
                ReadConfig();
            }
            return _configuredMessages;
        }

        public int GetConfiguredMessageIntervalInSeconds()
        {
            if(_configuredMessageInterval == null)
            {
                ReadConfig();
            }
            return _configuredMessageInterval ?? DEFAULT_MESSAGE_INTERVAL;
        }

        private void ReadConfig()
        {
            if (_configStream == null)
            {
                return;
            }

            XmlDocument document = new XmlDocument();
            document.Load(_configStream);

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
                _configuredMessages = new List<string>(messagesList.Count);
                foreach( XmlNode messageNode in messagesList)
                {
                    if (messageNode != null && !string.IsNullOrEmpty(messageNode.Value))
                    {
                        _configuredMessages.Add(messageNode.Value);
                    }
                }
            }
        }
    }
}
