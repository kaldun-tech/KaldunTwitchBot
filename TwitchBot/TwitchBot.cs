using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TwitchBot
{
	internal partial class TwitchBot : Form
	{
		public TwitchBot()
		{
			InitializeComponent();

			_connection = null;
			_fileDialog = new OpenFileDialog();
			_configReader = null;
			_automaticMessageSender = null;
			_drinkingIntroductions = new Dictionary<string, object>();
			// Use case-insensitive comparisons so viewers can target each other even if they don't
			// use the capitalization we expect.
			_drinkingParticipants = new Dictionary<string, DrinkingGameParticipant>(StringComparer.InvariantCultureIgnoreCase);
			_generator = new Random();
			_imageForm = new Images();
			_imageForm.VisibleChanged += new EventHandler(image_VisibilityChanged);
			_log = null;
			_raffleViewers = new Dictionary<string, object>();
			_viewers = new Dictionary<string, object>();
			_windowColor = textBoxR.BackColor;

			comboBoxCustomCharacter.Items.Add(1);
			comboBoxCustomCharacter.Items.Add(2);
			comboBoxCustomCharacter.Items.Add(3);
			comboBoxCustomCharacter.Items.Add(4);
			comboBoxCustomCharacter.SelectedIndex = 0;
		}

		private Connection _connection;
		// Configuration
		OpenFileDialog _fileDialog;
		ConfigurationReader _configReader;
		ConfigurableMessageSender _automaticMessageSender;
		// Contains all the viewers that have for sure seen the drinking game introduction message.
		private IDictionary<string, object> _drinkingIntroductions;
		/// <summary>
		/// Dictionary mapping usernames to DrinkingGameParticipant objects
		/// </summary>
		private IDictionary<string, DrinkingGameParticipant> _drinkingParticipants;
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

		private void DrinkingAdd(string viewer, string characterName)
		{
			int characterNum;
			if (characterName.Equals(textBoxCharacter1.Text, StringComparison.InvariantCultureIgnoreCase))
			{
				characterNum = 1;
			}
			else if (characterName.Equals(textBoxCharacter2.Text, StringComparison.InvariantCultureIgnoreCase))
			{
				characterNum = 2;
			}
			else if (characterName.Equals(textBoxCharacter3.Text, StringComparison.InvariantCultureIgnoreCase))
			{
				characterNum = 3;
			}
			else if (characterName.Equals(textBoxCharacter4.Text, StringComparison.InvariantCultureIgnoreCase))
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

			DrinkingGameParticipant info;
			if (!_drinkingParticipants.TryGetValue(viewer, out info))
			{
				info = new DrinkingGameParticipant(characterNum);
				comboBoxViewer.Items.Add(viewer.ToLowerInvariant());
				comboBoxCustom.Items.Add(viewer.ToLowerInvariant());
				_drinkingParticipants.Add(viewer.ToLowerInvariant(), info);
			}
			else
			{
				info.CharacterNum = characterNum;
			}
		}

		private void DrinkingGive(string source, string target)
		{
			if (!checkBoxPlay.Checked)
			{
				_connection.Send(string.Format("@{0}, we're not currently playing a drinking game.", source));
				return;
			}

			DrinkingGameParticipant sourceInfo;
			if (!_drinkingParticipants.TryGetValue(source, out sourceInfo) ||
				sourceInfo.Tickets < 1)
			{
				_connection.Send(string.Format("@{0}, you do not have any drink tickets to give.", source));
				return;
			}

			if (!_drinkingParticipants.ContainsKey(target))
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

		private bool TryGetMatch(Regex pattern, string line, out Match match)
		{
			match = pattern.Match(line);
			return match.Success;
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
				if (_automaticMessageSender != null)
				{
					_automaticMessageSender.Disconnect();
				}
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

			Regex raffle = new Regex("^!raffle$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			Regex join = new Regex("^!join (.*)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			Regex give = new Regex("^!give (.*)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			Regex quit = new Regex("^!quit$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

			Match match;

			if (TryGetMatch(raffle, e.Content, out match))
			{
				RaffleAdd(e.From);
			}
			else if (checkBoxPlay.Checked && TryGetMatch(join, e.Content, out match))
			{
				DrinkingAdd(e.From, match.Groups[1].Value);
				if (!_drinkingIntroductions.ContainsKey(e.From))
				{
					_drinkingIntroductions.Add(e.From, null);
				}
			}
			else if (checkBoxPlay.Checked && TryGetMatch(give, e.Content, out match))
			{
				DrinkingGive(e.From, match.Groups[1].Value);
				if (!_drinkingIntroductions.ContainsKey(e.From))
				{
					_drinkingIntroductions.Add(e.From, null);
				}
			}
			else if (TryGetMatch(quit, e.Content, out match))
			{
				comboBoxViewer.Items.Remove(e.From.ToLowerInvariant());
				comboBoxCustom.Items.Remove(e.From.ToLowerInvariant());
				_drinkingParticipants.Remove(e.From);
				if (!_drinkingIntroductions.ContainsKey(e.From))
				{
					_drinkingIntroductions.Add(e.From, null);
				}
			}

			if (checkBoxPlay.Checked && !_drinkingIntroductions.ContainsKey(e.From))
			{
				_connection.Send(string.Format("Welcome to the channel! We're playing a drinking game. If you want to join, type \"!join <character>\". Current characters are {0}, {1}, {2} and {3}. Type \"!quit\" to stop playing.",
					textBoxCharacter1.Text, textBoxCharacter2.Text, textBoxCharacter3.Text, textBoxCharacter4.Text));
				foreach (string user in _viewers.Keys)
				{
					if (!_drinkingIntroductions.ContainsKey(user))
					{
						_drinkingIntroductions.Add(user, null);
					}
				}
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

			comboBoxViewer.Items.Remove(e.User.ToLowerInvariant());
			comboBoxCustom.Items.Remove(e.User.ToLowerInvariant());
			_drinkingParticipants.Remove(e.User);
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

			// Configure the automatic message sender thread
			if (_configReader != null)
			{
				_automaticMessageSender = new ConfigurableMessageSender(_connection, _configReader.GetConfiguredMessageIntervalInSeconds(), _configReader.GetConfiguredMessages());
				_automaticMessageSender.Start();
			}

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
            if (_automaticMessageSender != null)
            {
                _automaticMessageSender.Disconnect();
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
				_connection.Send(string.Format("A drinking game has been started! Type \"!join <character>\" to play. Current characters are {0}, {1}, {2} and {3}. Type \"!quit\" to stop playing.",
					textBoxCharacter1.Text, textBoxCharacter2.Text, textBoxCharacter3.Text, textBoxCharacter4.Text));
				foreach (string user in _viewers.Keys)
				{
					if (!_drinkingIntroductions.ContainsKey(user))
					{
						_drinkingIntroductions.Add(user, null);
					}
				}
			}
			else
			{
				_connection.Send("The drinking game has ended.");
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

			StringBuilder messageTargets = new StringBuilder();
			foreach (KeyValuePair<string, DrinkingGameParticipant> viewerAssignment in _drinkingParticipants)
			{
				if (viewerAssignment.Value.CharacterNum == characterNum)
				{
					if (messageTargets.Length > 0)
					{
						messageTargets.Append(", @");
					}
					messageTargets.Append(viewerAssignment.Key);
				}
			}

			if (messageTargets.Length > 0)
			{
				_connection.Send(string.Format(Strings.TakeDrink, messageTargets));
			}
		}

		private void buttonViewerDrink_Click(object sender, EventArgs e)
		{
			checkBoxPlay.Checked = true;

			if (!_drinkingParticipants.ContainsKey(comboBoxViewer.Text))
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

			foreach (KeyValuePair<string, DrinkingGameParticipant> viewerAssignment in _drinkingParticipants)
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

			DrinkingGameParticipant info;
			if (!_drinkingParticipants.TryGetValue(comboBoxViewer.Text, out info))
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

			StringBuilder messageTargets = new StringBuilder();
			foreach (KeyValuePair<string, DrinkingGameParticipant> viewerAssignment in _drinkingParticipants)
			{
				if (viewerAssignment.Value.CharacterNum == characterNum)
				{
					if (messageTargets.Length > 0)
					{
						messageTargets.Append(", @");
					}
					messageTargets.Append(viewerAssignment.Key);
				}
			}

			if (messageTargets.Length > 0)
			{
				_connection.Send(string.Format(Strings.FinishDrink, messageTargets));
			}
		}

		private void buttonViewerFinish_Click(object sender, EventArgs e)
		{
			checkBoxPlay.Checked = true;

			if (!_drinkingParticipants.ContainsKey(comboBoxViewer.Text))
			{
				return;
			}

			_connection.Send(string.Format(Strings.FinishDrink, comboBoxViewer.Text));
		}

		private void buttonAllDrink_Click(object sender, EventArgs e)
		{
			checkBoxPlay.Checked = true;

			StringBuilder summary = new StringBuilder();

			foreach (string viewer in _drinkingParticipants.Keys)
			{
				summary.AppendFormat("@{0}, ", viewer);
			}
			summary.Append("Everyone drink!");

			_connection.Send(summary.ToString());
		}

		/// <summary>
		/// Manually add a drinking game participant as an administrative maintenance function.
		/// The new participant does not need to be a viewer, so this can be used to let people
		/// join the game without requiring them to be in the chat.
		/// </summary>
		private void buttonAdd_Click(object sender, EventArgs e)
		{
			string viewer = comboBoxCustom.Text.ToLowerInvariant();
			int characterNum = (int)comboBoxCustomCharacter.SelectedItem;

			comboBoxCustom.Text = "";

			DrinkingGameParticipant info;
			if (_drinkingParticipants.TryGetValue(viewer, out info))
			{
				info.CharacterNum = characterNum;
				return;
			}

			comboBoxViewer.Items.Add(viewer);
			comboBoxCustom.Items.Add(viewer);
			_drinkingParticipants.Add(viewer, new DrinkingGameParticipant(characterNum));
		}

		/// <summary>
		/// Manually remove a drinking game participant as an administrative maintenance
		/// function. It would be pretty rude to remove someone that is actually a viewer since
		/// they would lose all their drink tickets and they would have to re-join, so
		/// administrators should probably only remove people that they added themselves.
		/// </summary>
		private void buttonRemove_Click(object sender, EventArgs e)
		{
			if (!_drinkingParticipants.ContainsKey(comboBoxCustom.Text))
			{
				return;
			}

			comboBoxViewer.Items.Remove(comboBoxCustom.Text.ToLowerInvariant());
			comboBoxCustom.Items.Remove(comboBoxCustom.Text.ToLowerInvariant());
			_drinkingParticipants.Remove(comboBoxCustom.Text);
		}

		/// <summary>
		/// Update the character number combobox for the manually entered drinking game
		/// participant. This can be used to check the current character assignment of any
		/// participant.
		/// </summary>
		private void comboBoxCustom_SelectedIndexChanged(object sender, EventArgs e)
		{
			string viewer = (string)comboBoxCustom.SelectedItem;
			DrinkingGameParticipant info = _drinkingParticipants[viewer];
			comboBoxCustomCharacter.SelectedItem = info.CharacterNum;
		}

		/// <summary>
		/// Update the participant's character assignment.
		/// </summary>
		private void comboBoxCustomCharacter_SelectedIndexChanged(object sender, EventArgs e)
		{
			DrinkingGameParticipant info;
			if (!_drinkingParticipants.TryGetValue(comboBoxCustom.Text, out info))
			{
				return;
			}

			info.CharacterNum = (int)comboBoxCustomCharacter.SelectedItem;
		}

		private void ConfigFileButton_Click( object sender, EventArgs e )
		{
			_fileDialog.ShowDialog();
			if ( !string.IsNullOrEmpty( _fileDialog.FileName ) )
			{
                _configReader = new ConfigurationReader(_fileDialog.FileName);
            }
		}
	}
}
