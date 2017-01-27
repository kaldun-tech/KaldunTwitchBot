using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TwitchBot
{
	public class Connection : IDisposable
	{
		public Connection(string chat)
		{
			_channel = chat;
			_client = null;
			_connectSuccess = false;
			_connecting = new ManualResetEvent(false);
			_listener = null;
			_sender = null;
		}

		public event EventHandler Disconnected;

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
		private bool _connectSuccess;
		private ManualResetEvent _connecting;
		private Thread _listener;
		private ThrottledSender _sender;

		/// <summary>
		/// Start a background thread that connects and handles incoming traffic.
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <param name="oAuth"></param>
		/// <param name="useSSL"></param>
		public void Connect(string hostname, int port, bool useSSL, string user, string oAuth)
		{
			_listener = new Thread(delegate()
				{
					try
					{
						_client = new TcpClient(hostname, port);
					}
					catch (SocketException)
					{
						_connectSuccess = false;
						OnDisconnected();
						_connecting.Set();
						return;
					}

					Stream stream = _client.GetStream();
					if (useSSL)
					{
						SslStream ssl = new SslStream(stream, false);
						ssl.AuthenticateAsClient(hostname);
						stream = ssl;
					}

					Encoding utf8 = new UTF8Encoding(false, true);
					TextWriter writer = new StreamWriter(stream, utf8);
					TextReader reader = new StreamReader(stream, utf8);
					writer.WriteLine("PASS " + oAuth);
					writer.WriteLine("NICK " + user);
					writer.Flush();
					_sender = new ThrottledSender(20, new TimeSpan(0, 0, 30), writer);
					_sender.MessageSent += OnMessageSent;

					_connectSuccess = true;
					_connecting.Set();

					// Register for IRCv3 membership capability so we get notifications for people joining
					// and leaving.
					_sender.Send("CAP REQ :twitch.tv/membership", false);
					_sender.Send("JOIN #" + _channel, false);

					Listen(reader);
				});
			_listener.Name = "Connection";
			_listener.Start();
		}

		/// <summary>
		/// Add a chat message to the queue to be sent.
		/// </summary>
		public void Send(string text)
		{
			_connecting.WaitOne();
			if (!_connectSuccess)
			{
				return;
			}
			_sender.Send("PRIVMSG #" + _channel + " :" + text, true);
		}

		/// <summary>
		/// Add an IRC command message to the queue to be sent.
		/// </summary>
		public void SendRaw(string text)
		{
			_connecting.WaitOne();
			if (!_connectSuccess)
			{
				return;
			}
			_sender.Send(text, true);
		}

		public void Dispose()
		{
			if (_listener == null)
			{
				_connecting.Close();
				return;
			}

			_connecting.WaitOne();
			_connecting.Close();

			if (!_connectSuccess)
			{
				return;
			}

			_sender.Send("QUIT", true);
			_sender.RequestExit();
			_listener.Join();
			_sender.MessageSent -= OnMessageSent;
			_sender.Dispose();
			_client.Close();
		}

		/// <summary>
		/// Raise the Disconnected event.
		/// </summary>
		protected virtual void OnDisconnected()
		{
			if (Disconnected != null)
			{
				Disconnected(this, EventArgs.Empty);
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

		private void Listen(TextReader reader)
		{
			Regex ping = new Regex("^PING :(.+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
			Regex join = new Regex(":([^!]+)!\\1@\\1.tmi.twitch.tv JOIN #(.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
			Regex part = new Regex(":([^!]+)!\\1@\\1.tmi.twitch.tv PART #(.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
			Regex privMsg = new Regex(":([^!]+)!\\1@\\1.tmi.twitch.tv PRIVMSG #[^ ]* :(.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
			string line;
			while (TryReadLine(reader, out line))
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

			OnDisconnected();
		}

		private bool TryReadLine(TextReader reader, out string result)
		{
			try
			{
				result = reader.ReadLine();
			}
			catch (IOException)
			{
				result = null;
				return false;
			}

			return (result != null);
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
