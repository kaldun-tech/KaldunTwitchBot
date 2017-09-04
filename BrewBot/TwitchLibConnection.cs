using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using BrewBot.Commands;
using BrewBot.Interfaces;

namespace BrewBot
{
	internal class TwitchLibConnection
	{
		/// <summary>
		/// Create a new connection
		/// </summary>
		/// <param name="chat">Target IRC chat</param>
		public TwitchLibConnection( string hostname, int port, string chat, string username, string oauth, CommandFactory commandFactory, string subscriberTitle )
		{
			_channel = chat;
			_subscriberTitle = subscriberTitle;
			_credentials = new ConnectionCredentials( username, oauth, hostname, port );
			_commandFactory = commandFactory;

			_client = new TwitchClient( _credentials, _channel );
			_client.OnJoinedChannel += onJoinedChannel;
			_client.OnMessageReceived += onMessageReceived;
			_client.OnWhisperReceived += onWhisperReceived;
			_client.OnNewSubscriber += onNewSubscriber;
		}

		private readonly string _channel;
		private readonly string _subscriberTitle;
		private ConnectionCredentials _credentials;
		private TwitchClient _client;
		private CommandFactory _commandFactory;

		public void Connect()
		{
			_client.Connect();
		}

		public void Disconnect()
		{
			_client.Disconnect();
		}

		public void Send( string text )
		{
			if ( _client.IsConnected )
			{
				_client.SendMessage( text );
			}
		}

		public void SendRaw( string text )
		{
			if ( _client.IsConnected )
			{
				_client.SendRaw( text );
			}
		}

		private void onJoinedChannel( object sender, OnJoinedChannelArgs e )
		{
			Send( Strings.ChannelJoined );
		}

		private void onMessageReceived( object sender, OnMessageReceivedArgs e )
		{
			// TODO configure a list of banned words
			if ( e.ChatMessage.Message.Contains( "badword" ) )
			{
				// TODO time user out
			}
		}

		private void onCommandReceived( object sender, OnWhisperCommandReceivedArgs e )
		{
			if ( _commandFactory != null && _client.IsConnected )
			{
				ICommand command = _commandFactory.CreateCommand( e.WhisperMessage.Message, e.WhisperMessage.Username );
				if ( command != null )
				{
					command.ExecuteCommand();
				}
			}
		}

		private void onWhisperReceived( object sender, OnWhisperReceivedArgs e )
		{
			// We currently don't care about whispers
		}

		private void onNewSubscriber( object sender, OnNewSubscriberArgs e )
		{
			if ( e.Subscriber.IsTwitchPrime )
			{
				string message = string.Format( Strings.SubscriptionReceivedPrime, e.Subscriber.DisplayName, _subscriberTitle );
				Send( message );
			}
			else
			{
				string message = string.Format( Strings.SubscriptionReceived, e.Subscriber.DisplayName, _subscriberTitle );
				Send( message );
			}
		}
	}
}
