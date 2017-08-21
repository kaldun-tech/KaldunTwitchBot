using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using System;

namespace BrewBot
{
	internal class TwitchLibConnection
	{
		/// <summary>
		/// Create a new connection
		/// </summary>
		/// <param name="chat">Target IRC chat</param>
		public TwitchLibConnection( string hostname, int port, string chat, string username, string oauth )
		{
			_channel = chat;
			_credentials = new ConnectionCredentials( username, oauth, hostname, port );

			_client = new TwitchClient( _credentials, _channel );
			_client.OnJoinedChannel += onJoinedChannel;
			_client.OnMessageReceived += onMessageReceived;
			_client.OnWhisperReceived += onWhisperReceived;
			_client.OnNewSubscriber += onNewSubscriber;
		}

		private const string SUBSCRIBER_TITLE = "Brewster";

		private string _channel;
		private ConnectionCredentials _credentials;
		private TwitchClient _client;

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
			// TODO Strings.Resx
			Send( Strings.ChannelJoined );
		}

		private void onMessageReceived( object sender, OnMessageReceivedArgs e )
		{
			// TODO configure a list of naughty words
			if ( e.ChatMessage.Message.Contains( "badword" ) )
			{
				// TODO time user out
			}
		}

		private void onCommandReceived( object sender, OnWhisperCommandReceivedArgs e )
		{
			// TODO delegate to CommandFactory
		}

		private void onWhisperReceived( object sender, OnWhisperReceivedArgs e )
		{
			// TODO We currently don't care about whispers
		}

		private void onNewSubscriber( object sender, OnNewSubscriberArgs e )
		{
			// TODO Strings.Resx
			if ( e.Subscriber.IsTwitchPrime )
			{
				string message = string.Format( Strings.SubscriptionReceivedPrime, e.Subscriber.DisplayName, SUBSCRIBER_TITLE );
				Send( message );
			}
			else
			{
				string message = string.Format( Strings.SubscriptionReceived, e.Subscriber.DisplayName, SUBSCRIBER_TITLE );
				Send( message );
			}
		}
	}
}
