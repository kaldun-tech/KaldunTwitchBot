using System.Collections.Generic;
using System.Threading;
using BrewBot.Connection;

namespace BrewBot.Config
{
    internal class ConfigurableMessageSender
    {
		/// <summary>
		/// Create a new configurable message sender
		/// </summary>
		/// <param name="connection">Connection to use. Must not be null.</param>
		/// <param name="messageIntervalInSeconds">Interval between messages. Must be greater than or equal to zero.</param>
		/// <param name="configuredMessages">List of messages to send. Should be non-null of length greater than or equal to zero.</param>
        public ConfigurableMessageSender( TwitchLibConnection connection, int messageIntervalInSeconds, List<string> configuredMessages )
        {
            _connection = connection;
            _messageIntervalInSeconds = messageIntervalInSeconds;
            _configuredMessages = configuredMessages;
            _senderThread = new Thread( new ThreadStart( SendMessageLoop ) );
            _lock = new object();
        }

        private object _lock;
        private TwitchLibConnection _connection;
        private int _messageIntervalInSeconds;
        private List<string> _configuredMessages;
        private Thread _senderThread;

		// Send the messages in a loop
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

		/// <summary>
		/// Start the send thread
		/// </summary>
        public void Start()
        {
            if ( !_senderThread.IsAlive )
            {
                _senderThread.Start();
            }
        }

		/// <summary>
		/// Stop the send thread
		/// </summary>
        public void Stop()
        {
            _senderThread.Abort();
        }
    }
}