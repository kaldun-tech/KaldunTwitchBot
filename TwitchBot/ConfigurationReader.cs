using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TwitchBot
{
    internal class ConfigurationReader
    {
        public ConfigurationReader()
        {
            try
            {
                configFileStream = File.OpenRead(STANDARD_CONFIG_FILE_PATH);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public ConfigurationReader(string customPath)
        {
            try
            {
                configFileStream = File.OpenRead(customPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private const string STANDARD_CONFIG_FILE_PATH = "config\\config.xml";
        private const int DEFAULT_MESSAGE_INTERVAL = 120;
        private const int MINIMUM_MESSAGE_INTERVAL = 30;

        private FileStream configFileStream = null;
        private List<string> configuredMessages = null;
        private int? configuredMessageInterval = null;

        public List<string> GetConfiguredMessages()
        {
            if (configuredMessages == null)
            {
                ReadConfig();
            }
            return configuredMessages;
        }

        public int GetConfiguredMessageIntervalInSeconds()
        {
            if(configuredMessageInterval == null)
            {
                ReadConfig();
            }
            return configuredMessageInterval ?? DEFAULT_MESSAGE_INTERVAL;
        }

        private void ReadConfig()
        {
            if (configFileStream == null)
            {
                return;
            }

            XmlDocument document = new XmlDocument();
            document.Load(configFileStream);

            XmlNode intervalNode = document.DocumentElement.SelectSingleNode("/config/interval");
            XmlAttributeCollection attributes;
            XmlNode waitTimeNode;
            if (intervalNode != null && (attributes = intervalNode.Attributes) != null && (waitTimeNode = attributes.GetNamedItem("wait-time")) != null)
            {
                string value = waitTimeNode.Value;
                configuredMessageInterval = int.Parse(value);
                if (configuredMessageInterval < MINIMUM_MESSAGE_INTERVAL)
                {
                    configuredMessageInterval = MINIMUM_MESSAGE_INTERVAL;
                }
            }
            else
            {
                configuredMessageInterval = DEFAULT_MESSAGE_INTERVAL;
            }

            XmlNodeList messagesList = document.DocumentElement.SelectNodes("/config/messages/message");
            if (messagesList != null)
            {
                configuredMessages = new List<string>(messagesList.Count);
                foreach( XmlNode messageNode in messagesList)
                {
                    if (messageNode != null && !string.IsNullOrEmpty(messageNode.Value))
                    {
                        configuredMessages.Add(messageNode.Value);
                    }
                }
            }
        }
    }
}
