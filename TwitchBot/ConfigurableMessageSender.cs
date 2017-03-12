using System.Collections.Generic;
using System.Threading;

namespace TwitchBot
{
    internal class ConfigurableMessageSender
    {
        public ConfigurableMessageSender(Connection connection, int messageIntervalInSeconds, List<string> configuredMessages)
        {
            _connection = connection;
            _messageIntervalInSeconds = messageIntervalInSeconds;
            _configuredMessages = configuredMessages;
        }

        private Connection _connection;
        private int _messageIntervalInSeconds;
        private List<string> _configuredMessages;

        public void Start()
        {
            if (_messageIntervalInSeconds < 0 || _configuredMessages == null || _configuredMessages.Count <= 0)
            {
                return;
            }

            int messageIntervalInMillis = _messageIntervalInSeconds * 1000;
            int currentIndex = 0;
            while (_connection != null)
            {
                string nextMessage = _configuredMessages[currentIndex];
                ++currentIndex;
                currentIndex %= _configuredMessages.Count;

                if (string.IsNullOrEmpty(nextMessage))
                {
                    continue;
                }

                // Send with lowest priority, no big deal if we're late
                lock (this)
                {
                    if ( _connection != null)
                    {
                        _connection.Send(nextMessage);
                    }
                }
                Thread.Sleep(messageIntervalInMillis);
            }
        }

        public void Disconnect()
        {
            lock (this)
            {
                _connection = null;
            }
        }
    }
}
