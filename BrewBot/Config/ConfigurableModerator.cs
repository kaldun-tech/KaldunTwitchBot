using BrewBot.Connection;
using System.Collections.Generic;

namespace BrewBot.Config
{
	class ConfigurableModerator
	{
		public ConfigurableModerator( int timeoutTimeSeconds, List<string> timeoutWords, List<string> bannedWords, TwitchLibConnection channelConnection )
		{
			_timeoutSeconds = timeoutTimeSeconds;
			_timeoutWords = timeoutWords;
			_bannedWords = bannedWords;
			_channelConnection = channelConnection;
		}

		private int _timeoutSeconds;
		private List<string> _timeoutWords;
		private List<string> _bannedWords;
		TwitchLibConnection _channelConnection;

		/// <summary>
		/// Check whether the moderator is connected
		/// </summary>
		public bool IsConnected
		{
			get { return _channelConnection != null && _channelConnection.IsConnected; }
		}

		/// <summary>
		/// Timeout and ban users as appropriate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="message"></param>
		public void ScrubMessage( string sender, string message )
		{
			if ( IsConnected )
			{
				foreach ( string timeoutWord in _timeoutWords )
				{
					if ( message.Contains( timeoutWord ) )
					{
						_channelConnection.TimeoutUser( sender, _timeoutSeconds );
					}
				}
				foreach ( string bannedWord in _bannedWords )
				{
					if ( message.Contains( bannedWord ) )
					{
						_channelConnection.BanUser( sender );
					}
				}
			}
		}

		/// <summary>
		/// Disconnect from a chat channel
		/// </summary>
		public void Disconnect()
		{
			_channelConnection = null;
		}

		/// <summary>
		/// Reconnect to a chat channel
		/// </summary>
		/// <param name="connection"></param>
		public void Reconnect( TwitchLibConnection connection )
		{
			_channelConnection = connection;
		}
	}
}
