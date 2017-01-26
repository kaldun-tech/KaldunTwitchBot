using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TwitchBot
{
	public partial class TwitchBot : Form
	{
		public TwitchBot()
		{
			InitializeComponent();

			_connection = null;
			_generator = new Random();
			_imageForm = new Images();
			_imageForm.VisibleChanged += new EventHandler(image_VisibilityChanged);
			_log = null;
			_raffleUsers = new Dictionary<string, object>();
			_windowColor = textBoxR.BackColor;
		}

		private Connection _connection;
		private Random _generator;
		private Images _imageForm;
		private TextWriter _log;
		private IDictionary<string, object> _raffleUsers;
		private Color _windowColor;

		private void StartLog(bool append)
		{
			saveFileDialogLog.OverwritePrompt = !append;
			switch (saveFileDialogLog.ShowDialog(this))
			{
				case DialogResult.OK:
					break;
				case DialogResult.Cancel:
					return;
				default:
					throw new Exception();
			}

			if (_log != null)
			{
				_log.Dispose();
			}

			_log = new StreamWriter(saveFileDialogLog.FileName, append);
			_log.Write(textBoxLog.Text);
		}

		private void ConnectionMessageTransfer(object sender, Connection.MessageEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new Connection.MessageEventHandler(ConnectionMessageTransfer), sender, e);
				return;
			}

			string toAppend = DateTime.Now.ToString("HH:mm:ss.f") + (e.IsReceived ? " >" : " <") + e.Text;

			textBoxLog.Text += toAppend + Environment.NewLine;
			if (scrollToNewMessageToolStripMenuItem.Checked)
			{
				textBoxLog.Select(textBoxLog.Text.Length, 0);
				textBoxLog.ScrollToCaret();
			}

			if (_log != null)
			{
				_log.WriteLine(toAppend);
			}
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
			string hostname = "irc.chat.twitch.tv";

			_connection = new Connection(textBoxChat.Text);
			_connection.MessageReceived += ConnectionMessageTransfer;
			_connection.MessageSent += ConnectionMessageTransfer;
			_connection.PrivateMessageReceived += ConnectionPrivateMessageReceived;
			_connection.Connect(hostname, 443, true, textBoxUser.Text, textBoxPassword.Text);
		}

		private void buttonSendRaw_Click(object sender, EventArgs e)
		{
			if (_connection == null)
			{
				return;
			}
			_connection.SendRaw(textBoxInput.Text);
			textBoxInput.Clear();
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			if (_connection == null)
			{
				return;
			}
			_connection.Send(textBoxInput.Text);
			textBoxInput.Clear();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		// Clean up here since the Dispose method is implemented in the designer code.
		private void TwitchBot_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_connection != null)
			{
				_connection.Dispose();
			}
			if (_log != null)
			{
				_log.Dispose();
				_log = null;
			}
		}

		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textBoxLog.Clear();
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

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StartLog(false);
		}

		private void appendToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StartLog(true);
		}

		private void logToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!logToolStripMenuItem.Checked)
			{
				return;
			}

			_log.Dispose();
			_log = null;
			logToolStripMenuItem.Checked = false;
		}

		/// <summary>
		/// Update the menu based on which tab is currently selected.
		/// </summary>
		private void tabControlConnected_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool isTrafficSelected = (tabControlConnected.SelectedTab == tabPageTraffic);
			editToolStripMenuItem.Visible = isTrafficSelected;
			logToolStripMenuItem.Visible = isTrafficSelected;
			scrollToNewMessageToolStripMenuItem.Visible = isTrafficSelected;
		}
	}
}
