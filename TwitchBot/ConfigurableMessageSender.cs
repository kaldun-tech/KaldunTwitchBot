using System.Collections.Generic;
using System.Threading;

namespace TwitchBot
{
    internal class ConfigurableMessageSender
    {
        public ConfigurableMessageSender( Connection connection, int messageIntervalInSeconds, List<string> configuredMessages )
        {
            _connection = connection;
            _messageIntervalInSeconds = messageIntervalInSeconds;
            _configuredMessages = configuredMessages;
            _senderThread = new Thread( new ThreadStart( SendMessageLoop ) );
            _lock = new object();
        }

        private object _lock;
        private Connection _connection;
        private int _messageIntervalInSeconds;
        private List<string> _configuredMessages;
        private Thread _senderThread;

        private void SendMessageLoop()
        {
            try
            {
                // Loop and send messages until disconnected
                if ( _messageIntervalInSeconds < 0 || _configuredMessages == null || _configuredMessages.Count <= 0 )
                {
                    return;
                }

                int messageIntervalInMillis = _messageIntervalInSeconds * 1000;
                int currentIndex = 0;
                while ( true )
                {
                    string nextMessage = _configuredMessages[ currentIndex ];
                    ++currentIndex;
                    currentIndex %= _configuredMessages.Count;

                    if ( string.IsNullOrEmpty( nextMessage ) )
                    {
                        continue;
                    }

                    _connection.Send( nextMessage );
                    Thread.Sleep( messageIntervalInMillis );
                }
            }
            catch ( ThreadInterruptedException )
            { }
        }

        public void Start()
        {
            if ( !_senderThread.IsAlive )
            {
                _senderThread.Start();
            }
        }

        public void Disconnect()
        {
            _senderThread.Abort();
        }
    }
}