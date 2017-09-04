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
		public TwitchLibConnection( string hostname, int port, string chat, string username, string oauth, string subscriberTitle, CommandFactory commandFactory, UserManager userManager )
		{
			_channel = chat;
			_subscriberTitle = subscriberTitle;
			_credentials = new ConnectionCredentials( username, oauth, hostname, port );
			_commandFactory = commandFactory;
			_userManager = userManager;

			_client = new TwitchClient( _credentials, _channel );
			_client.OnJoinedChannel += OnJoinedChannel;
			_client.OnMessageReceived += OnMessageReceived;
			_client.OnWhisperReceived += OnWhisperReceived;
			_client.OnNewSubscriber += OnNewSubscriber;
			_client.OnUserJoined += OnUserJoined;
			_client.OnUserLeft += OnUserLeft;
		}

		private readonly string _channel;
		private readonly string _subscriberTitle;
		private ConnectionCredentials _credentials;
		private TwitchClient _client;
		private CommandFactory _commandFactory;
		private UserManager _userManager;

		/// <summary>
		/// Connect to the Twitch server
		/// </summary>
		public void Connect()
		{
			_client.Connect();
		}

		/// <summary>
		/// Disconnect from the twitch server
		/// </summary>
		public void Disconnect()
		{
			_client.Disconnect();
		}

		/// <summary>
		/// Sends an IRC message.
		/// </summary>
		/// <param name="text"></param>
		public void Send( string text )
		{
			if ( _client.IsConnected )
			{
				_client.SendMessage( text );
			}
		}

		/// <summary>
		/// Sends a RAW IRC message.
		/// </summary>
		/// <param name="text"></param>
		public void SendRaw( string text )
		{
			if ( _client.IsConnected )
			{
				_client.SendRaw( text );
			}
		}

		private void OnJoinedChannel( object sender, OnJoinedChannelArgs e )
		{
			Send( Strings.ChannelJoined );
		}

		private void OnMessageReceived( object sender, OnMessageReceivedArgs e )
		{
			// TODO configure a list of banned words
			if ( e.ChatMessage.Message.Contains( "badword" ) )
			{
				// TODO time user out
			}
		}

		private void OnCommandReceived( object sender, OnWhisperCommandReceivedArgs e )
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

		private void OnWhisperReceived( object sender, OnWhisperReceivedArgs e )
		{
			// We currently don't care about whispers
		}

		private void OnNewSubscriber( object sender, OnNewSubscriberArgs e )
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

		private void OnUserJoined( object sender, OnUserJoinedArgs e )
		{
			_userManager.LoginUser( e.Username );
		}

		private void OnUserLeft( object sender, OnUserLeftArgs e )
		{
			_userManager.LogoutUser( e.Username );
		}
	}
}
