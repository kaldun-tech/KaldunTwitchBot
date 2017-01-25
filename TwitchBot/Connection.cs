using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TwitchBot
{
	public class Connection : IDisposable
	{
		public Connection(string user, string chat)
		{
			_channel = chat;
			_client = new TcpClient();
			_disposeLock = new object();
			_reader = null;
			_sender = null;
			_user = user;
		}

		/// <summary>
		/// Occurs when any traffic is received from the server, including internal IRC protocol.
		/// </summary>
		public event MessageEventHandler MessageReceived;

		/// <summary>
		/// Occurs when any traffic is sent actually sent to the server, not when it is added to
		/// queue to be sent.
		/// </summary>
		public event MessageEventHandler MessageSent;

		/// <summary>
		/// Occurs when a user sends a message to the chat. It is a 'private' message as far as the
		/// IRC protocal is concerned, not because it is sent to a particular user.
		/// </summary>
		public event PrivateMessageReceivedEventHandler PrivateMessageReceived;

		/// <summary>
		/// Occurs when a user joins the chat.
		/// </summary>
		public event UserEventHandler UserJoined;

		/// <summary>
		/// Occurs when a user leaves the chat.
		/// </summary>
		public event UserEventHandler UserLeft;

		private string _channel;
		private TcpClient _client;
		private object _disposeLock;
		private TextReader _reader;
		private ThrottledSender _sender;
		private string _user;

		public void Connect(string hostname, int port, string oAuth, bool useSSL)
		{
			_client.Connect(hostname, port);
			Stream stream = _client.GetStream();
			if (useSSL)
			{
				SslStream ssl = new SslStream(_client.GetStream(), false);
				ssl.AuthenticateAsClient(hostname);
				stream = ssl;
			}

			Encoding utf8 = new UTF8Encoding(false, true);
			TextWriter writer = new StreamWriter(stream, utf8);
			_reader = new StreamReader(stream, utf8);
			_sender = new ThrottledSender(20, new TimeSpan(0, 0, 30), writer);
			_sender.MessageSent += OnMessageSent;
			_sender.Send("PASS " + oAuth, false);
			_sender.Send("NICK " + _user, false);
			// Register for IRCv3 membership capability so we get notifications for people joining
			// and leaving.
			_sender.Send("CAP REQ :twitch.tv/membership", false);
			_sender.Send("JOIN #" + _channel, false);
		}

		/// <summary>
		/// Add a chat message to the queue to be sent.
		/// </summary>
		public void Send(string text)
		{
			_sender.Send("PRIVMSG #" + _channel + " :" + text, true);
		}

		/// <summary>
		/// Add an IRC command message to the queue to be sent.
		/// </summary>
		public void SendRaw(string text)
		{
			_sender.Send(text, true);
		}

		/// <summary>
		/// Begin the main listener loop to handle incoming traffic. Blocks until the connection is closed.
		/// </summary>
		public void DoWork()
		{
			Regex ping = new Regex("^PING :(.+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
			Regex join = new Regex(":([^!]+)!\\1@\\1.tmi.twitch.tv JOIN #(.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
			Regex part = new Regex(":([^!]+)!\\1@\\1.tmi.twitch.tv PART #(.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
			Regex privMsg = new Regex(":([^!]+)!\\1@\\1.tmi.twitch.tv PRIVMSG #[^ ]* :(.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
			string line;
			while ((line = Receive()) != null)
			{
				OnMessageReceived(line);

				Match pingMatch = ping.Match(line);
				if (pingMatch.Success)
				{
					_sender.Send("PONG :" + pingMatch.Groups[1].Value, true);
					continue;
				}

				Match privMsgMatch = privMsg.Match(line);
				if (privMsgMatch.Success)
				{
					OnPrivateMessageReceived(privMsgMatch.Groups[1].Value, privMsgMatch.Groups[2].Value);
					continue;
				}

				Match joinMatch = join.Match(line);
				if (joinMatch.Success)
				{
					OnUserJoined(joinMatch.Groups[1].Value);
					continue;
				}

				Match partMatch = part.Match(line);
				if (partMatch.Success)
				{
					OnUserLeft(partMatch.Groups[1].Value);
					continue;
				}
			}
		}

		public void Dispose()
		{
			_sender.Send("QUIT", true);
			_sender.RequestExit();
			_sender.MessageSent -= OnMessageSent;
			_sender.Dispose();
			_sender = null;
			lock (_disposeLock)
			{
				if (_client != null)
				{
					_client.Close();
					_client = null;
				}
			}
		}

		/// <summary>
		/// Raise the MessageReceived event.
		/// </summary>
		/// <param name="text">The traffic that was received.</param>
		protected virtual void OnMessageReceived(string text)
		{
			if (MessageReceived != null)
			{
				MessageEventArgs e = new MessageEventArgs(text, true);
				MessageReceived(this, e);
			}
		}

		/// <summary>
		/// Raise the MessageSent event.
		/// </summary>
		/// <param name="sender">The original sender of the event. Ignored because we are
		/// forwarding it, so we are the new sender.</param>
		protected virtual void OnMessageSent(object sender, MessageEventArgs e)
		{
			if (MessageSent != null)
			{
				MessageSent(this, e);
			}
		}

		/// <summary>
		/// Raise the PrivateMessageReceived event.
		/// </summary>
		/// <param name="from">The user sending the message.</param>
		/// <param name="content">The content of the message.</param>
		protected virtual void OnPrivateMessageReceived(string from, string content)
		{
			if (PrivateMessageReceived != null)
			{
				PrivateMessageReceivedEventArgs e = new PrivateMessageReceivedEventArgs(from, content);
				PrivateMessageReceived(this, e);
			}
		}

		/// <summary>
		/// Raise the UserJoined event.
		/// </summary>
		/// <param name="user">The user that joined the chat.</param>
		protected virtual void OnUserJoined(string user)
		{
			if (UserJoined != null)
			{
				UserEventArgs e = new UserEventArgs(user);
				UserJoined(this, e);
			}
		}

		/// <summary>
		/// Raise the UserLeft event.
		/// </summary>
		/// <param name="user">The user that left the chat.</param>
		protected virtual void OnUserLeft(string user)
		{
			if (UserLeft != null)
			{
				UserEventArgs e = new UserEventArgs(user);
				UserLeft(this, e);
			}
		}

		private string Receive()
		{
			string retval;
			lock (_disposeLock)
			{
				if (_client == null)
				{
					return null;
				}
				retval = _reader.ReadLine();
			}
			return retval;
		}

		public delegate void MessageEventHandler(object sender, MessageEventArgs e);
		public delegate void PrivateMessageReceivedEventHandler(object sender, PrivateMessageReceivedEventArgs e);
		public delegate void UserEventHandler(object sender, UserEventArgs e);

		public class MessageEventArgs : EventArgs
		{
			public MessageEventArgs(string text, bool isReceived)
			{
				_isReceived = isReceived;
				_text = text;
			}

			private bool _isReceived;
			private string _text;

			/// <summary>
			/// Whether the message was received. Otherwise, it was sent.
			/// </summary>
			public bool IsReceived
			{
				get { return _isReceived; }
			}

			public string Text
			{
				get { return _text; }
			}
		}

		public class PrivateMessageReceivedEventArgs : EventArgs
		{
			public PrivateMessageReceivedEventArgs(string from, string content)
			{
				_content = content;
				_from = from;
			}

			private string _content;
			private string _from;

			public string Content
			{
				get { return _content; }
			}

			public string From
			{
				get { return _from; }
			}
		}

		public class UserEventArgs : EventArgs
		{
			public UserEventArgs(string user)
			{
				_user = user;
			}

			private string _user;

			public string User
			{
				get { return _user; }
			}
		}
	}
}
