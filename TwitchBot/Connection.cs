using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TwitchBot
{
	public class Connection : IDisposable
	{
		public Connection(string hostname, int port, string user, string oAuth, string chat, ShowTextDelegate showText)
		{
			_channel = chat;
			Encoding utf8 = new UTF8Encoding(false, true);
			_client = new TcpClient(hostname, port);
			_disposeLock = new object();
			Stream stream = _client.GetStream();
			TextWriter writer = new StreamWriter(stream, utf8);
			_reader = new StreamReader(stream, utf8);
			_sender = new ThrottledSender(20, new TimeSpan(0, 0, 30), showText, writer);
			_sender.Send("PASS " + oAuth, false);
			_sender.Send("NICK " + user, false);
			// Register for IRCv3 membership capability so we get notifications for people joining
			// and leaving.
			_sender.Send("CAP REQ :twitch.tv/membership", false);
			_sender.Send("JOIN #" + chat, false);
			_user = user;
		}

		public event MessageReceivedEventHandler MessageReceived;
		public event PrivateMessageReceivedEventHandler PrivateMessageReceived;

		private string _channel;
		private TcpClient _client;
		private object _disposeLock;
		private TextReader _reader;
		private ThrottledSender _sender;
		private string _user;

		public void Send(string text)
		{
			_sender.Send("PRIVMSG #" + _channel + " :" + text, true);
		}

		public void SendRaw(string text)
		{
			_sender.Send(text, true);
		}

		public void DoWork()
		{
			Regex ping = new Regex("^PING :(.+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
			Regex join = new Regex(":([^!]+)!\\1@\\1.tmi.twitch.tv JOIN #(.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
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
					continue;
				}
			}
		}

		public void Dispose()
		{
			_sender.Send("QUIT", true);
			_sender.RequestExit();
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

		protected virtual void OnMessageReceived(string text)
		{
			if (MessageReceived != null)
			{
				MessageReceivedEventArgs e = new MessageReceivedEventArgs(text);
				MessageReceived(this, e);
			}
		}

		protected virtual void OnPrivateMessageReceived(string from, string content)
		{
			if (PrivateMessageReceived != null)
			{
				PrivateMessageReceivedEventArgs e = new PrivateMessageReceivedEventArgs(from, content);
				PrivateMessageReceived(this, e);
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

		public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
		public delegate void PrivateMessageReceivedEventHandler(object sender, PrivateMessageReceivedEventArgs e);

		public class MessageReceivedEventArgs : EventArgs
		{
			public MessageReceivedEventArgs(string text)
			{
				_text = text;
			}

			private string _text;

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
	}
}
