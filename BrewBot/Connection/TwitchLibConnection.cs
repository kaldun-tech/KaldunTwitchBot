using System;
using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;

namespace BrewBot.Connection
{
	internal class TwitchLibConnection
	{
		/// <summary>
		/// Create a new connection
		/// </summary>
		/// <param name="chat">Target IRC chat</param>
		/// <param name="username"></param>
		/// <param name="oauth"></param>
		public TwitchLibConnection( string chat, string username, string oauth )
		{
			_channel = chat;
			_credentials = new ConnectionCredentials( username, oauth );
			_client = new TwitchClient( _credentials, _channel );

			OnConnected += ConnectedCB;
			OnJoinedChannel += JoinedChannelCB;
		}

		public EventHandler<OnConnectedArgs> OnConnected;
		public EventHandler<OnDisconnectedArgs> OnDisconnected;
		public EventHandler<OnJoinedChannelArgs> OnJoinedChannel;
		public EventHandler<OnMessageReceivedArgs> OnMessageReceived;
		public EventHandler<OnMessageSentArgs> OnMessageSent;
		public EventHandler<OnWhisperReceivedArgs> OnWhisperReceived;
		public EventHandler<OnNewSubscriberArgs> OnNewSubscriber;
		public EventHandler<OnUserJoinedArgs> OnUserJoined;
		public EventHandler<OnUserLeftArgs> OnUserLeft;

		private readonly string _channel;
		private ConnectionCredentials _credentials;
		private TwitchClient _client;


		public bool IsConnected
		{
			get { return _client.IsConnected; }
		}

		/// <summary>
		/// Connect to the Twitch server
		/// </summary>
		public void Connect()
		{
			_client.OnConnected += OnConnected;
			_client.OnDisconnected += OnDisconnected;
			_client.OnJoinedChannel += OnJoinedChannel;
			_client.OnMessageReceived += OnMessageReceived;
			_client.OnMessageSent += OnMessageSent;
			_client.OnWhisperReceived += OnWhisperReceived;
			_client.OnNewSubscriber += OnNewSubscriber;
			_client.OnUserJoined += OnUserJoined;
			_client.OnUserLeft += OnUserLeft;

			_client.Connect();
		}

		/// <summary>
		/// Disconnect from the twitch server
		/// </summary>
		public void Disconnect()
		{
			_client.OnConnected -= OnConnected;
			_client.OnJoinedChannel -= OnJoinedChannel;
			_client.OnMessageReceived -= OnMessageReceived;
			_client.OnMessageSent -= OnMessageSent;
			_client.OnWhisperReceived -= OnWhisperReceived;
			_client.OnNewSubscriber -= OnNewSubscriber;
			_client.OnUserJoined -= OnUserJoined;
			_client.OnUserLeft -= OnUserLeft;

			_client.LeaveChannel( _channel );
			_client.Disconnect();
		}

		/// <summary>
		/// Sends an IRC message.
		/// </summary>
		/// <param name="text"></param>
		public void Send( string text )
		{
			if ( _client.IsConnected && !string.IsNullOrEmpty( text ) )
			{
				_client.SendMessage( text );
			}
		}

		/// <summary>
		/// Sends a whisper to another user in IRC
		/// </summary>
		/// <param name="receiver">Target user</param>
		/// <param name="message"></param>
		public void SendWhisper( string receiver, string message )
		{
			_client.SendWhisper( receiver, message );
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

		private void ConnectedCB( object sender, OnConnectedArgs e )
		{
			_client.JoinChannel( _channel );
		}

		private void JoinedChannelCB( object sender, OnJoinedChannelArgs e )
		{
			Send( Strings.ChannelJoined );
		}
	}
}
