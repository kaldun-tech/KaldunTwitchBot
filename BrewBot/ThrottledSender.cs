using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BrewBot
{
    internal class ThrottledSender : IDisposable
    {
        public ThrottledSender( int messageLimit, TimeSpan period, TextWriter writer )
        {
            _exit = false;
            _history = new List<DateTime>();
            _limit = messageLimit;
            _period = period;
            _queue = new PriorityQueue<Message>();
            _sender = new Thread( DoSend );
            _sender.Name = "Sender";
            _signal = new AutoResetEvent( false );
            _writer = writer;
            _sender.Start();
        }

        /// <summary>
        /// Occurs when any traffic is actually sent to the server, not when it is added to
        /// queue to be sent.
        /// </summary>
        public event Connection.MessageEventHandler MessageSent;

        private bool _exit;
        private IList<DateTime> _history;
        private int _limit;
        private TimeSpan _period;
        private PriorityQueue<Message> _queue;
        private Thread _sender;
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

        public void Send( string text, int priority, bool display )
        {
            lock ( _queue )
            {
                _queue.Add( new Message( text, priority, display ) );
                _signal.Set();
            }
        }

        public void Dispose()
        {
            _sender.Join();
            _signal.Close();
        }

        protected virtual void OnMessageSent( string text )
        {
            if ( MessageSent != null )
            {
                Connection.MessageEventArgs e = new Connection.MessageEventArgs( text, false );
                MessageSent( this, e );
            }
        }

        private void DoSend()
        {
            while ( !_exit )
            {
                _signal.WaitOne();
                while ( !_exit && SendItems() )
                {
                    Thread.Sleep( _history[ 0 ].Add( _period ).Subtract( DateTime.UtcNow ) );
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
            lock ( _queue )
            {
                while ( _history.Count > 0 && DateTime.UtcNow.Subtract( _period ).CompareTo( _history[ 0 ] ) > 0 )
                {
                    _history.RemoveAt( 0 );
                }
                while ( _history.Count < _limit && _queue.Count > 0 )
                {
                    Message msg = _queue.Remove();
                    _writer.WriteLine( msg.Content );
                    try
                    {
                        _writer.Flush();
                    }
                    catch ( IOException )
                    {
                        _exit = true;
                        return true;
                    }
                    if ( msg.Display )
                    {
                        OnMessageSent( msg.Content );
                    }
                    _history.Add( DateTime.UtcNow );
                }
                if ( _queue.Count == 0 )
                {
                    itemsRemain = false;
                }
            }
            return itemsRemain;
        }

        private class Message : IComparable<Message>
        {
            public Message( string content, int priority, bool display )
            {
                _content = content;
                _display = display;
                _priority = priority;
            }

            private string _content;
            private bool _display;
            private int _priority;

            public string Content
            {
                get { return _content; }
            }

            public bool Display
            {
                get { return _display; }
            }

            public int CompareTo( Message other )
            {
                return _priority - other._priority;
            }
        }
    }
}