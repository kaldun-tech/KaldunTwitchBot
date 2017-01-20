using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace TwitchBot
{
	public delegate void ShowTextDelegate(string text, bool isReceived);

	public partial class TwitchBot : Form
	{
		public TwitchBot()
		{
			InitializeComponent();

			_connection = null;
			_connectionLock = new object();
			_generator = new Random();
			_imageForm = new Images();
			_imageForm.VisibleChanged += new EventHandler(image_VisibilityChanged);
			_isConnecting = false;
			_raffleUsers = new Dictionary<string, object>();
			_windowColor = textBoxR.BackColor;
		}

		private Connection _connection;
		private object _connectionLock;
		private Random _generator;
		private Images _imageForm;
		private bool _isConnecting;
		private IDictionary<string, object> _raffleUsers;
		private Color _windowColor;

		private void SaveLog()
		{
			using (TextWriter writer = File.CreateText(saveFileDialogLog.FileName))
			{
				writer.Write(textBoxLog.Text);
			}
		}

		private void ShowText(string text, bool isReceived)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new ShowTextDelegate(ShowText), text, isReceived);
				return;
			}

			textBoxLog.Text += DateTime.Now.ToString("HH:mm:ss.f") + (isReceived ? " >" : " <") + text + Environment.NewLine;
			if (scrollToNewMessageToolStripMenuItem.Checked)
			{
				textBoxLog.Select(textBoxLog.Text.Length, 0);
				textBoxLog.ScrollToCaret();
			}
		}

		private bool UpdateLogFile()
		{
			switch (saveFileDialogLog.ShowDialog(this))
			{
				case DialogResult.OK:
					return true;
				case DialogResult.Cancel:
					return false;
				default:
					throw new Exception();
			}
		}

		private void ConnectionMessageReceived(object sender, Connection.MessageReceivedEventArgs e)
		{
			ShowText(e.Text, true);
		}

		private void ConnectionPrivateMessageReceived(object sender, Connection.PrivateMessageReceivedEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new Connection.PrivateMessageReceivedEventHandler(ConnectionPrivateMessageReceived), sender, e);
				return;
			}

			Regex raffle = new Regex("^!raffle$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

			Match raffleMatch = raffle.Match(e.Content);
			if (raffleMatch.Success)
			{
				if (_raffleUsers.ContainsKey(e.From))
				{
					return;
				}

				listBoxRaffle.Items.Add(e.From);
				_raffleUsers.Add(e.From, null);
				return;
			}
		}

		private void buttonDoWork_Click(object sender, EventArgs e)
		{
			lock (_connectionLock)
			{
				if (_isConnecting)
				{
					return;
				}
				_isConnecting = true;
			}

			Thread handleReceive = new Thread(delegate()
				{
					string hostname = "irc.chat.twitch.tv";
					lock (_connectionLock)
					{
						if (!_isConnecting)
						{
							return;
						}
						_connection = new Connection(hostname, 6667, textBoxUser.Text, textBoxPassword.Text, textBoxChat.Text, new ShowTextDelegate(ShowText));
						_connection.MessageReceived += ConnectionMessageReceived;
						_connection.PrivateMessageReceived += ConnectionPrivateMessageReceived;
						_isConnecting = false;
					}
					_connection.DoWork();
				});
			handleReceive.Name = "Connection";
			handleReceive.Start();
		}

		private void buttonSendRaw_Click(object sender, EventArgs e)
		{
			lock (_connectionLock)
			{
				if (_isConnecting || _connection == null)
				{
					return;
				}
			}

			_connection.SendRaw(textBoxInput.Text);
			textBoxInput.Clear();
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			lock (_connectionLock)
			{
				if (_isConnecting || _connection == null)
				{
					return;
				}
			}

			_connection.Send(textBoxInput.Text);
			textBoxInput.Clear();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void TwitchBot_FormClosed(object sender, FormClosedEventArgs e)
		{
			lock (_connectionLock)
			{
				if (_isConnecting)
				{
					_isConnecting = false;
				}
				else if (_connection != null)
				{
					_connection.Dispose();
					_connection = null;
				}
			}
		}

		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textBoxLog.Clear();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (saveFileDialogLog.FileName == string.Empty &&
				!UpdateLogFile())
			{
				return;
			}

			SaveLog();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!UpdateLogFile())
			{
				return;
			}

			SaveLog();
		}

		private void imageWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_imageForm.Visible = !imageWindowToolStripMenuItem.Checked;
		}

		private void image_VisibilityChanged(object sender, EventArgs e)
		{
			imageWindowToolStripMenuItem.Checked = ((Images)sender).Visible;
		}

		private void textBoxRGB_TextChanged(object sender, EventArgs e)
		{
			bool allValid = true;
			byte r, g, b;

			if (byte.TryParse(textBoxR.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out r))
			{
				textBoxR.BackColor = _windowColor;
			}
			else
			{
				allValid = false;
				textBoxR.BackColor = Color.Red;
			}

			if (byte.TryParse(textBoxG.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out g))
			{
				textBoxG.BackColor = _windowColor;
			}
			else
			{
				allValid = false;
				textBoxG.BackColor = Color.Red;
			}

			if (byte.TryParse(textBoxB.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out b))
			{
				textBoxB.BackColor = _windowColor;
			}
			else
			{
				allValid = false;
				textBoxB.BackColor = Color.Red;
			}

			if (!allValid)
			{
				return;
			}

			_imageForm.BackColor = Color.FromArgb(r, g, b);
		}

		private void buttonClear_Click(object sender, EventArgs e)
		{
			_raffleUsers.Clear();
			listBoxRaffle.Items.Clear();
		}

		private void buttonDraw_Click(object sender, EventArgs e)
		{
			if (_raffleUsers.Count < 1)
			{
				return;
			}

			listBoxRaffle.SelectedIndex = _generator.Next(_raffleUsers.Count);
		}
	}
}
