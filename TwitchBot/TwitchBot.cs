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
			_drunkestIntroductions = new Dictionary<string, object>();
			// Use case-insensitive comparisons so viewers can target each other even if they don't
			// use correct capitalization.
			_drunkestParticipants = new Dictionary<string, DrunkestViewer>(StringComparer.InvariantCultureIgnoreCase);
			_generator = new Random();
			_imageForm = new Images();
			_imageForm.VisibleChanged += new EventHandler(image_VisibilityChanged);
			_log = null;
			_raffleViewers = new Dictionary<string, object>();
			_viewers = new Dictionary<string, object>();
			_windowColor = textBoxR.BackColor;
		}

		private Connection _connection;
		// Contains all the viewers that have for sure seen the drunkest dungeon introduction message.
		private IDictionary<string, object> _drunkestIntroductions;
		private IDictionary<string, DrunkestViewer> _drunkestParticipants;
		private Random _generator;
		private Images _imageForm;
		private TextWriter _log;
		private IDictionary<string, object> _raffleViewers;
		// Contains the viewers that are probably in the channel right now. Values are all null.
		private IDictionary<string, object> _viewers;
		private Color _windowColor;

		private void RaffleAdd(string viewer)
		{
			if (_raffleViewers.ContainsKey(viewer))
			{
				return;
			}

			listBoxRaffle.Items.Add(viewer);
			_raffleViewers.Add(viewer, null);
		}

		private void DrunkestAdd(string viewer, string characterName)
		{
			int characterNum;
			if (characterName == textBoxCharacter1.Text)
			{
				characterNum = 1;
			}
			else if (characterName == textBoxCharacter2.Text)
			{
				characterNum = 2;
			}
			else if (characterName == textBoxCharacter3.Text)
			{
				characterNum = 3;
			}
			else if (characterName == textBoxCharacter4.Text)
			{
				characterNum = 4;
			}
			else
			{
				_connection.Send(string.Format("@{0}, choose from {1}, {2}, {3} or {4}.",
					viewer, textBoxCharacter1.Text, textBoxCharacter2.Text,
					textBoxCharacter3.Text, textBoxCharacter4.Text));
				return;
			}

			DrunkestViewer info;
			if (!_drunkestParticipants.TryGetValue(viewer, out info))
			{
				info = new DrunkestViewer(characterNum);
				comboBoxViewer.Items.Add(viewer);
				_drunkestParticipants.Add(viewer, info);
			}
			else
			{
				info.CharacterNum = characterNum;
			}
		}

		private void DrunkestGive(string source, string target)
		{
			if (!checkBoxPlay.Checked)
			{
				_connection.Send(string.Format("@{0}, we're not currently playing Drunkest Dungeon.", source));
				return;
			}

			DrunkestViewer sourceInfo;
			if (!_drunkestParticipants.TryGetValue(source, out sourceInfo) ||
				sourceInfo.Tickets < 1)
			{
				_connection.Send(string.Format("@{0}, you do not have any drink tickets to give.", source));
				return;
			}

			if (!_drunkestParticipants.ContainsKey(target))
			{
				_connection.Send(string.Format("@{0}, {1} is not participating.", source, target));
				return;
			}

			sourceInfo.Tickets -= 1;
			_connection.Send(string.Format(Strings.TakeDrink, target));
		}

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

		private void HandleConnectionChange(bool isConnect)
		{
			panelDisconnected.Enabled = !isConnect;
			panelDisconnected.Visible = !isConnect;
			tabControlConnected.Visible = isConnect;
			tabControlConnected.Enabled = isConnect;
			disconnectToolStripMenuItem.Enabled = isConnect;
			disconnectToolStripMenuItem.Visible = isConnect;
			HandleTabChange(isConnect ? tabControlConnected.SelectedTab : null);
		}

		/// <summary>
		/// Update the menu based on which tab is currently selected.
		/// </summary>
		/// <param name="selected">The newly-selected tab, or null if the tab control is disabled.</param>
		private void HandleTabChange(TabPage selected)
		{
			bool isTrafficSelected = (selected == tabPageTraffic);
			appendToolStripMenuItem.Enabled = isTrafficSelected;
			appendToolStripMenuItem.Visible = isTrafficSelected;
			clearToolStripMenuItem.Enabled = isTrafficSelected;
			clearToolStripMenuItem.Visible = isTrafficSelected;
			editToolStripMenuItem.Enabled = isTrafficSelected;
			editToolStripMenuItem.Visible = isTrafficSelected;
			logToolStripMenuItem.Enabled = isTrafficSelected;
			logToolStripMenuItem.Visible = isTrafficSelected;
			newToolStripMenuItem.Enabled = isTrafficSelected;
			newToolStripMenuItem.Visible = isTrafficSelected;
			scrollToNewMessageToolStripMenuItem.Enabled = isTrafficSelected;
			scrollToNewMessageToolStripMenuItem.Visible = isTrafficSelected;
		}

		private void ConnectionDisconnected(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new EventHandler(ConnectionDisconnected), sender, e);
				return;
			}

			if (_connection != null)
			{
				_connection.Disconnected -= ConnectionDisconnected;
				_connection.MessageReceived -= ConnectionMessageTransfer;
				_connection.MessageSent -= ConnectionMessageTransfer;
				_connection.PrivateMessageReceived -= ConnectionPrivateMessageReceived;
				_connection.UserJoined -= ConnectionUserJoined;
				_connection.UserLeft -= ConnectionUserLeft;
				_connection.Dispose();
				_connection = null;
			}

			HandleConnectionChange(false);
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

			// We may not have gotten a join notification for this user yet.
			if (!_viewers.ContainsKey(e.From))
			{
				_viewers.Add(e.From, null);
			}

			if (checkBoxPlay.Checked && !_drunkestIntroductions.ContainsKey(e.From))
			{
				_connection.Send(string.Format("Welcome to the channel! We're playing Drunkest Dungeon. If you want to join, type \"!join <character>\". Current characters are {0}, {1}, {2} and {3}. Type \"!quit\" to stop playing.",
					textBoxCharacter1.Text, textBoxCharacter2.Text, textBoxCharacter3.Text, textBoxCharacter4.Text));
				foreach (string user in _viewers.Keys)
				{
					if (!_drunkestIntroductions.ContainsKey(user))
					{
						_drunkestIntroductions.Add(user, null);
					}
				}
			}

			Regex raffle = new Regex("^!raffle$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			Regex join = new Regex("^!join (.*)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			Regex give = new Regex("^!give (.*)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			Regex quit = new Regex("^!quit$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

			Match raffleMatch = raffle.Match(e.Content);
			if (raffleMatch.Success)
			{
				RaffleAdd(e.From);
				return;
			}

			Match joinMatch = join.Match(e.Content);
			if (joinMatch.Success)
			{
				DrunkestAdd(e.From, joinMatch.Groups[1].Value);
				return;
			}

			Match giveMatch = give.Match(e.Content);
			if (giveMatch.Success)
			{
				DrunkestGive(e.From, giveMatch.Groups[1].Value);
				return;
			}

			Match quitMatch = quit.Match(e.Content);
			if (quitMatch.Success)
			{
				_drunkestParticipants.Remove(e.From);
				return;
			}
		}

		private void ConnectionUserJoined(object sender, Connection.UserEventArgs e)
		{
			if (!_viewers.ContainsKey(e.User))
			{
				_viewers.Add(e.User, null);
			}
		}

		private void ConnectionUserLeft(object sender, Connection.UserEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new Connection.UserEventHandler(ConnectionUserLeft), sender, e);
				return;
			}

			_viewers.Remove(e.User);

			comboBoxViewer.Items.Remove(e.User);
			_drunkestParticipants.Remove(e.User);
		}

		private void buttonDoWork_Click(object sender, EventArgs e)
		{
			if (_connection != null)
			{
				_connection.Disconnected -= ConnectionDisconnected;
				_connection.MessageReceived -= ConnectionMessageTransfer;
				_connection.MessageSent -= ConnectionMessageTransfer;
				_connection.PrivateMessageReceived -= ConnectionPrivateMessageReceived;
				_connection.UserJoined -= ConnectionUserJoined;
				_connection.UserLeft -= ConnectionUserLeft;
				_connection.Dispose();
			}

			string hostname = "irc.chat.twitch.tv";

			_connection = new Connection(textBoxChat.Text);
			_connection.Disconnected += ConnectionDisconnected;
			_connection.MessageReceived += ConnectionMessageTransfer;
			_connection.MessageSent += ConnectionMessageTransfer;
			_connection.PrivateMessageReceived += ConnectionPrivateMessageReceived;
			_connection.UserJoined += ConnectionUserJoined;
			_connection.UserLeft += ConnectionUserLeft;
			_connection.Connect(hostname, checkBoxSSL.Checked ? 443 : 6667, checkBoxSSL.Checked, textBoxUser.Text, textBoxPassword.Text);

			HandleConnectionChange(true);
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
				_connection.Disconnected -= ConnectionDisconnected;
				_connection.MessageReceived -= ConnectionMessageTransfer;
				_connection.MessageSent -= ConnectionMessageTransfer;
				_connection.PrivateMessageReceived -= ConnectionPrivateMessageReceived;
				_connection.UserJoined -= ConnectionUserJoined;
				_connection.UserLeft -= ConnectionUserLeft;
				_connection.Dispose();
				_connection = null;
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
			_raffleViewers.Clear();
			listBoxRaffle.Items.Clear();
		}

		private void buttonDraw_Click(object sender, EventArgs e)
		{
			if (_raffleViewers.Count < 1)
			{
				return;
			}

			listBoxRaffle.SelectedIndex = _generator.Next(_raffleViewers.Count);
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

		private void tabControlConnected_SelectedIndexChanged(object sender, EventArgs e)
		{
			HandleTabChange(tabControlConnected.SelectedTab);
		}

		private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_connection == null)
			{
				return;
			}

			_connection.Disconnected -= ConnectionDisconnected;
			_connection.MessageReceived -= ConnectionMessageTransfer;
			_connection.MessageSent -= ConnectionMessageTransfer;
			_connection.PrivateMessageReceived -= ConnectionPrivateMessageReceived;
			_connection.UserJoined -= ConnectionUserJoined;
			_connection.UserLeft -= ConnectionUserLeft;
			_connection.Dispose();
			_connection = null;
			HandleConnectionChange(false);
		}

		private void checkBoxPlay_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxPlay.Checked)
			{
				_connection.Send(string.Format("A game of Drunkest Dungeon has been started! Type \"!join <character>\" to play. Current characters are {0}, {1}, {2} and {3}. Type \"!quit\" to stop playing.",
					textBoxCharacter1.Text, textBoxCharacter2.Text, textBoxCharacter3.Text, textBoxCharacter4.Text));
				foreach (string user in _viewers.Keys)
				{
					if (!_drunkestIntroductions.ContainsKey(user))
					{
						_drunkestIntroductions.Add(user, null);
					}
				}
			}
			else
			{
				_connection.Send("The game of Drunkest Dungeon has ended.");
			}
		}

		private void buttonCharacterDrink_Click(object sender, EventArgs e)
		{
			checkBoxPlay.Checked = true;

			int characterNum;
			if (sender == buttonCharacter1Drink)
			{
				characterNum = 1;
			}
			else if (sender == buttonCharacter2Drink)
			{
				characterNum = 2;
			}
			else if (sender == buttonCharacter3Drink)
			{
				characterNum = 3;
			}
			else if (sender == buttonCharacter4Drink)
			{
				characterNum = 4;
			}
			else
			{
				return;
			}

			foreach (KeyValuePair<string, DrunkestViewer> viewerAssignment in _drunkestParticipants)
			{
				if (viewerAssignment.Value.CharacterNum == characterNum)
				{
					_connection.Send(string.Format(Strings.TakeDrink, viewerAssignment.Key));
				}
			}
		}

		private void buttonViewerDrink_Click(object sender, EventArgs e)
		{
			checkBoxPlay.Checked = true;

			if (!_drunkestParticipants.ContainsKey(comboBoxViewer.Text))
			{
				return;
			}

			_connection.Send(string.Format(Strings.TakeDrink, comboBoxViewer.Text));
		}

		private void buttonCharacterGetTicket_Click(object sender, EventArgs e)
		{
			checkBoxPlay.Checked = true;

			int characterNum;
			if (sender == buttonCharacter1GetTicket)
			{
				characterNum = 1;
			}
			else if (sender == buttonCharacter2GetTicket)
			{
				characterNum = 2;
			}
			else if (sender == buttonCharacter3GetTicket)
			{
				characterNum = 3;
			}
			else if (sender == buttonCharacter4GetTicket)
			{
				characterNum = 4;
			}
			else
			{
				return;
			}

			foreach (KeyValuePair<string, DrunkestViewer> viewerAssignment in _drunkestParticipants)
			{
				if (viewerAssignment.Value.CharacterNum == characterNum)
				{
					viewerAssignment.Value.Tickets += 1;
					_connection.Send(string.Format(Strings.GetDrinkTicket, viewerAssignment.Key, viewerAssignment.Value.Tickets));
				}
			}
		}

		private void buttonViewerGetTicket_Click(object sender, EventArgs e)
		{
			checkBoxPlay.Checked = true;

			DrunkestViewer info;
			if (!_drunkestParticipants.TryGetValue(comboBoxViewer.Text, out info))
			{
				return;
			}

			info.Tickets += 1;
			_connection.Send(string.Format(Strings.GetDrinkTicket, comboBoxViewer.Text, info.Tickets));
		}

		private void buttonCharacterFinish_Click(object sender, EventArgs e)
		{
			checkBoxPlay.Checked = true;

			int characterNum;
			if (sender == buttonCharacter1Finish)
			{
				characterNum = 1;
			}
			else if (sender == buttonCharacter2Finish)
			{
				characterNum = 2;
			}
			else if (sender == buttonCharacter3Finish)
			{
				characterNum = 3;
			}
			else if (sender == buttonCharacter4Finish)
			{
				characterNum = 4;
			}
			else
			{
				return;
			}

			foreach (KeyValuePair<string, DrunkestViewer> viewerAssignment in _drunkestParticipants)
			{
				if (viewerAssignment.Value.CharacterNum == characterNum)
				{
					_connection.Send(string.Format(Strings.FinishDrink, viewerAssignment.Key));
				}
			}
		}

		private void buttonViewerFinish_Click(object sender, EventArgs e)
		{
			checkBoxPlay.Checked = true;

			if (!_drunkestParticipants.ContainsKey(comboBoxViewer.Text))
			{
				return;
			}

			_connection.Send(string.Format(Strings.FinishDrink, comboBoxViewer.Text));
		}
	}
}
