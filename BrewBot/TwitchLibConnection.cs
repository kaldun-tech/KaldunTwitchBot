using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using System;

namespace BrewBot
{
	internal class TwitchLibConnection : IDisposable, IConnection
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
		}

		/// <summary>
		/// Occurs when the bot is disconnected
		/// </summary>
		public event EventHandler Disconnected;

		/// <summary>
		/// Occurs when any traffic is received from the server, including internal IRC protocol.
		/// </summary>
		public event Connection.MessageEventHandler MessageReceived;

		/// <summary>
		/// Occurs when any traffic is sent actually sent to the server, not when it is added to
		/// queue to be sent.
		/// </summary>
		public event Connection.MessageEventHandler MessageSent;

		/// <summary>
		/// Occurs when a user sends a message to the chat. It is a 'private' message as far as the
		/// IRC protocal is concerned, not because it is sent to a particular user.
		/// </summary>
		public event Connection.PrivateMessageReceivedEventHandler PrivateMessageReceived;

		/// <summary>
		/// Occurs when a user joins the chat.
		/// </summary>
		public event Connection.UserEventHandler UserJoined;

		/// <summary>
		/// Occurs when a user leaves the chat.
		/// </summary>
		public event Connection.UserEventHandler UserLeft;

		private string _channel;
		private ConnectionCredentials _credentials;
		private TwitchClient _client;

		public void Connect( string hostname, int port, bool useSSL, string user, string oAuth )
		{
			throw new NotImplementedException();
		}

		public void Connect()
		{
			// TODO use SSL?
			_client.Connect();
		}

		public void Dispose()
		{
			_client.Disconnect();
		}

		public void Send( string text )
		{
			throw new NotImplementedException();
		}

		public void SendRaw( string text )
		{
			throw new NotImplementedException();
		}

		private void onJoinedChannel( object sender, OnJoinedChannelArgs e )
		{
			_client.SendMessage( "HeyGuys BrewBot ready for action!" );
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
			if ( e.Command == "help" )
			{
				_client.SendMessage( $"Hi there {e.WhisperMessage.Username}! You can view all commands using !command" );
			}
		}

		private void onWhisperReceived( object sender, OnWhisperReceivedArgs e )
		{
			// TODO We currently don't care about whispers
		}

		private void onNewSubscriber( object sender, OnNewSubscriberArgs e )
		{
			if ( e.Subscriber.IsTwitchPrime )
			{
				_client.SendMessage( $"Welcome {e.Subscriber.DisplayName} to the substers! So kind of you to use your Twitch Prime on this channel!" );
			}
			else
			{
				_client.SendMessage( $"Welcome {e.Subscriber.DisplayName} to the substers!" );
			}
		}
	}
}
