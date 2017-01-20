using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace TwitchBot
{
	public class ThrottledSender : IDisposable
	{
		public ThrottledSender(int messageLimit, TimeSpan period, ShowTextDelegate showText, TextWriter writer)
		{
			_exit = false;
			_history = new List<DateTime>();
			_limit = messageLimit;
			_period = period;
			_queue = new List<string>();
			_sender = new Thread(DoSend);
			_sender.Name = "Throttled Sender";
			_showText = showText;
			_signal = new AutoResetEvent(false);
			_writer = writer;
			_sender.Start();
		}

		private bool _exit;
		private IList<DateTime> _history;
		private int _limit;
		private TimeSpan _period;
		private IList<string> _queue;
		private Thread _sender;
		private ShowTextDelegate _showText;
		private AutoResetEvent _signal;
		private TextWriter _writer;

		/// <summary>
		/// Stop the sender after sending all messages that can be sent immediately.
		/// </summary>
		public void RequestExit()
		{
			_exit = true;
			_signal.Set();
		}

		public void Send(string text, bool isHighPriority)
		{
			lock (_queue)
			{
				if (isHighPriority)
				{
					// TODO: Make this a priority queue so we're not pre-empting other high priority items.
					_queue.Insert(0, text);
				}
				else
				{
					_queue.Add(text);
				}
				_signal.Set();
			}
		}

		public void Dispose()
		{
			_sender.Join();
			_signal.Close();
		}

		private void DoSend()
		{
			while (!_exit)
			{
				_signal.WaitOne();
				while (SendItems())
				{
					Thread.Sleep(_history[0].Add(_period).Subtract(DateTime.UtcNow));
				}
			}
		}

		/// <summary>
		/// Send items in the queue until it is empty or we've reached the throttling limit.
		/// </summary>
		/// <returns>Whether there are items remaining in the queue to send.</returns>
		private bool SendItems()
		{
			bool itemsRemain = true;
			lock (_queue)
			{
				while (_history.Count > 0 && DateTime.UtcNow.Subtract(_period).CompareTo(_history[0]) > 0)
				{
					_history.RemoveAt(0);
				}
				while (_history.Count < _limit && _queue.Count > 0)
				{
					string text = _queue[0];
					_queue.RemoveAt(0);
					_writer.WriteLine(text);
					_writer.Flush();
					_showText(text, false);
					_history.Add(DateTime.UtcNow);
				}
				if (_queue.Count == 0)
				{
					itemsRemain = false;
				}
			}
			return !_exit && itemsRemain;
		}
	}
}
