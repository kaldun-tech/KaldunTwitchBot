using System;

namespace BrewBot
{
	internal interface IConnection
	{
		event EventHandler Disconnected;
		event Connection.MessageEventHandler MessageReceived;
		event Connection.MessageEventHandler MessageSent;
		event Connection.PrivateMessageReceivedEventHandler PrivateMessageReceived;
		event Connection.UserEventHandler UserJoined;
		event Connection.UserEventHandler UserLeft;

		void Connect( string hostname, int port, bool useSSL, string user, string oAuth );
		void Dispose();
		void Send( string text );
		void SendRaw( string text );
	}
}