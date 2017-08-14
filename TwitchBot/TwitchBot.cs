using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using TwitchBot.Interfaces;
using TwitchBot.Commands;

namespace TwitchBot
{
	internal partial class TwitchBot : Form
    {
		/// <summary>
		/// Create a TwitchBot. This does all the UI logic
		/// </summary>
        public TwitchBot()
        {
            InitializeComponent();

            _connection = null;
            _fileDialog = new OpenFileDialog();
            _configReader = null;
            _automaticMessageSender = null;
            _imageForm = new Images();
            _imageForm.VisibleChanged += new EventHandler( image_VisibilityChanged );
            _log = null;
            _windowColor = textBoxR.BackColor;

			_commandFactory = new CommandFactory( GetBalanceCB, GambleCB, GiveDrinksCB, JoinCB, QuitCB, RaffleCB, SplashCB, GetTicketsCB, GetTotalDrinksCB );
			_credentialsReaderWriter = new LoginCredentialReaderWriter();
			_userManager = new UserManager();
			_drinkingGame = new DrinkingGame( _userManager );
			_raffle = new Raffle();

			comboBoxCustomCharacter.Items.Add( 1 );
            comboBoxCustomCharacter.Items.Add( 2 );
            comboBoxCustomCharacter.Items.Add( 3 );
            comboBoxCustomCharacter.Items.Add( 4 );
            comboBoxCustomCharacter.SelectedIndex = 0;
        }

		private readonly CommandFactory _commandFactory;
		private readonly LoginCredentialReaderWriter _credentialsReaderWriter;
		private readonly UserManager _userManager;
		private readonly DrinkingGame _drinkingGame;
		private readonly Raffle _raffle;

		private Connection _connection;
        // Configuration
        OpenFileDialog _fileDialog;
		string _configFilePath = null;
        ConfigurationReader _configReader;
        ConfigurableMessageSender _automaticMessageSender;
		Casino _casino;

        private Images _imageForm;
        private TextWriter _log;
		private Color _windowColor;

        private void RaffleAdd( string username )
        {
			if ( !_raffle.IsUserEntered( username ) )
            {
				listBoxRaffle.Items.Add( username );
				_raffle.AddUser( username );
			}
        }

        private void DrinkingAdd( string viewer, string characterName )
        {
            int characterNum;
            if ( characterName.Equals( textBoxCharacter1.Text, StringComparison.InvariantCultureIgnoreCase ) )
            {
                characterNum = 1;
            }
            else if ( characterName.Equals( textBoxCharacter2.Text, StringComparison.InvariantCultureIgnoreCase ) )
            {
                characterNum = 2;
            }
            else if ( characterName.Equals( textBoxCharacter3.Text, StringComparison.InvariantCultureIgnoreCase ) )
            {
                characterNum = 3;
            }
            else if ( characterName.Equals( textBoxCharacter4.Text, StringComparison.InvariantCultureIgnoreCase ) )
            {
                characterNum = 4;
            }
            else
            {
                _connection.Send( string.Format( Strings.ChooseCharacter,
                    viewer, textBoxCharacter1.Text, textBoxCharacter2.Text,
                    textBoxCharacter3.Text, textBoxCharacter4.Text ) );
                return;
            }

			string username = viewer.ToLowerInvariant();
			if ( _drinkingGame.IsUserPlaying( username ) )
			{
				comboBoxViewer.Items.Add( username );
				comboBoxCustom.Items.Add( username );
			}
			_drinkingGame.SetParticipant( username, characterNum );
        }

        private void StartLog( bool append )
        {
            saveFileDialogLog.OverwritePrompt = !append;
            switch ( saveFileDialogLog.ShowDialog( this ) )
            {
                case DialogResult.OK:
                    break;
                case DialogResult.Cancel:
                    return;
                default:
                    throw new Exception();
            }

            if ( _log != null )
            {
                _log.Dispose();
            }

            _log = new StreamWriter( saveFileDialogLog.FileName, append );
            _log.Write( textBoxLog.Text );
        }

        private void HandleConnectionChange( bool isConnect )
        {
            panelDisconnected.Enabled = !isConnect;
            panelDisconnected.Visible = !isConnect;
            tabControlConnected.Visible = isConnect;
            tabControlConnected.Enabled = isConnect;
            disconnectToolStripMenuItem.Enabled = isConnect;
            disconnectToolStripMenuItem.Visible = isConnect;
            HandleTabChange( isConnect ? tabControlConnected.SelectedTab : null );
        }

        /// <summary>
        /// Update the menu based on which tab is currently selected.
        /// </summary>
        /// <param name="selected">The newly-selected tab, or null if the tab control is disabled.</param>
        private void HandleTabChange( TabPage selected )
        {
            bool isTrafficSelected = ( selected == tabPageTraffic );
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

        private void ConnectionDisconnected( object sender, EventArgs e )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new EventHandler( ConnectionDisconnected ), sender, e );
                return;
            }

            if ( _connection != null )
            {
                if ( _automaticMessageSender != null )
                {
                    _automaticMessageSender.Stop();
                }
                if ( _casino != null )
                {
                    _casino.Stop();
				}
				if ( _userManager != null )
				{
					_userManager.Dispose();
				}
				if ( _drinkingGame != null )
				{
					_drinkingGame.Disconnect();
				}

				_connection.Disconnected -= ConnectionDisconnected;
                _connection.MessageReceived -= ConnectionMessageTransfer;
                _connection.MessageSent -= ConnectionMessageTransfer;
                _connection.PrivateMessageReceived -= ConnectionPrivateMessageReceived;
                _connection.UserJoined -= ConnectionUserJoined;
                _connection.UserLeft -= ConnectionUserLeft;
                _connection.Dispose();
                _connection = null;
            }

            HandleConnectionChange( false );
        }

        private void ConnectionMessageTransfer( object sender, Connection.MessageEventArgs e )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new Connection.MessageEventHandler( ConnectionMessageTransfer ), sender, e );
                return;
            }

            string toAppend = DateTime.Now.ToString( "HH:mm:ss.f" ) + ( e.IsReceived ? " >" : " <" ) + e.Text;

            textBoxLog.Text += toAppend + Environment.NewLine;
            if ( scrollToNewMessageToolStripMenuItem.Checked )
            {
                textBoxLog.Select( textBoxLog.Text.Length, 0 );
                textBoxLog.ScrollToCaret();
            }

            if ( _log != null )
            {
                _log.WriteLine( toAppend );
            }
        }

        private void GetBalanceCB( string from, string target )
        {
            string message = ( _casino == null ) ? "The casino is not currently operating, kupo!" : _casino.GetStringBalance( from );
			_connection.Send( message );
        }

        private void GambleCB( string from, string target )
        {
            string message = null;
            if ( _casino == null )
            {
                message = string.Format( Strings.Casino_NotOperating, from );
            }
            else
            {
                // Target is the gamble amount
                int betAmount = 0;
                if ( int.TryParse( target, out betAmount ) && betAmount > 0 )
                {
					message = _casino.Gamble( from, (uint) betAmount );
				}
                else
                {
                    message = string.Format( Strings.Casino_InvalidBetAmount, from );
                }
            }
            _connection.Send( message );
        }

        private void GiveDrinksCB( string from, string target )
        {
			_drinkingGame.GivePlayerDrink( from, target );
			_drinkingGame.AddIntroducedUser( from );
        }

        private void JoinCB( string from, string target )
        {
            DrinkingAdd( from, target );
			_drinkingGame.AddIntroducedUser( from );
        }

        private void QuitCB( string from, string target )
        {
            string fromToLower = from.ToLowerInvariant();
            comboBoxViewer.Items.Remove( fromToLower );
            comboBoxCustom.Items.Remove( fromToLower );
			_drinkingGame.RemoveParticipant( from );
			_drinkingGame.AddIntroducedUser( from );
        }

        private void RaffleCB( string from, string target )
        {
            RaffleAdd( from );
        }

        // TODO: We need the concept of an admin so randos can't just be splashing
        private void SplashCB( string from, string target )
        {
            // Target is the splash amount
            int splashAmount = 0;
            if ( _casino != null && int.TryParse(target, out splashAmount) )
            {
                _casino.SplashUsers( (uint) splashAmount );
            }
            else
            {
                _connection.Send( "Splash failed!" );
            }
        }

		private void GetTicketsCB( string from, string target )
		{
			uint drinkTickets = _userManager.GetDrinkTickets( from );
			string message = string.Format( Strings.DrinkTicketsBalance, from, drinkTickets );
			_connection.Send( message );
		}

		private void GetTotalDrinksCB( string from, string target )
		{
			uint numberOfDrinks = _userManager.GetNumberOfDrinksTaken( from );
			string message = string.Format( Strings.TotalDrinksTaken, from, numberOfDrinks );
			_connection.Send( message );
		}

        private void ConnectionPrivateMessageReceived( object sender, Connection.PrivateMessageReceivedEventArgs e )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new Connection.PrivateMessageReceivedEventHandler( ConnectionPrivateMessageReceived ), sender, e );
                return;
            }

            // We may not have gotten a join notification for this user yet.
            if ( !_userManager.IsUserActive( e.From ) )
            {
				_userManager.LoginUser( e.From );
            }

            ICommand command = _commandFactory.CreateCommand( e.Content, e.From );
            if ( command != null )
            {
                command.ExecuteCommand();
            }

			_drinkingGame.IntroduceUser( e.From, textBoxCharacter1.Text, textBoxCharacter2.Text, textBoxCharacter3.Text, textBoxCharacter4.Text );
        }

        private void ConnectionUserJoined( object sender, Connection.UserEventArgs e )
        {
			_userManager.LoginUser( e.User );
        }

        private void ConnectionUserLeft( object sender, Connection.UserEventArgs e )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new Connection.UserEventHandler( ConnectionUserLeft ), sender, e );
                return;
            }

			_userManager.LogoutUser( e.User );

			comboBoxViewer.Items.Remove( e.User.ToLowerInvariant() );
            comboBoxCustom.Items.Remove( e.User.ToLowerInvariant() );
            _drinkingGame.RemoveParticipant( e.User );
        }

        private void buttonDoWork_Click( object sender, EventArgs e )
        {
            if ( _connection != null )
            {
				_drinkingGame.Disconnect();
                _connection.Disconnected -= ConnectionDisconnected;
                _connection.MessageReceived -= ConnectionMessageTransfer;
                _connection.MessageSent -= ConnectionMessageTransfer;
                _connection.PrivateMessageReceived -= ConnectionPrivateMessageReceived;
                _connection.UserJoined -= ConnectionUserJoined;
                _connection.UserLeft -= ConnectionUserLeft;
                _connection.Dispose();
            }

            string hostname = "irc.chat.twitch.tv";
			string username = textBoxUser.Text;
			string password = textBoxPassword.Text;
			string channel = textBoxChat.Text;
			_configReader = new ConfigurationReader( _configFilePath );

			_connection = new Connection( channel );
            _connection.Disconnected += ConnectionDisconnected;
            _connection.MessageReceived += ConnectionMessageTransfer;
            _connection.MessageSent += ConnectionMessageTransfer;
            _connection.PrivateMessageReceived += ConnectionPrivateMessageReceived;
            _connection.UserJoined += ConnectionUserJoined;
            _connection.UserLeft += ConnectionUserLeft;
            _connection.Connect( hostname, checkBoxSSL.Checked ? 443 : 6667, checkBoxSSL.Checked, username, password );
			_drinkingGame.Connect( _connection );

			if ( saveCredentialsCheckbox.Checked )
			{
				_credentialsReaderWriter.WriteCredentials( username, password, channel, _configFilePath );
			}

            // Configure the automatic message sender thread
            if ( _configReader != null )
            {
                _automaticMessageSender = new ConfigurableMessageSender( _connection, _configReader.GetConfiguredMessageIntervalInSeconds, _configReader.GetConfiguredMessages );
                _automaticMessageSender.Start();
                if ( _configReader.IsGamblingEnabled )
                {
                    _casino = new Casino( _userManager, _configReader.CurrencyName, _configReader.CurrencyEarnedPerMinute, _configReader.MinimumGambleAmount, _configReader.ChanceToWin );
                    _casino.Start();
                }
            }
			// Reconnect the user manager
			if ( _userManager != null )
			{
				_userManager.Reconnect();
			}

            HandleConnectionChange( true );
        }

        private void buttonSendRaw_Click( object sender, EventArgs e )
        {
            if ( _connection == null )
            {
                return;
            }
            _connection.SendRaw( textBoxInput.Text );
            textBoxInput.Clear();
        }

        private void buttonSend_Click( object sender, EventArgs e )
        {
            if ( _connection == null )
            {
                return;
            }
            _connection.Send( textBoxInput.Text );
            textBoxInput.Clear();
        }

        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            Close();
        }

        // Clean up here since the Dispose method is implemented in the designer code.
        private void TwitchBot_FormClosed( object sender, FormClosedEventArgs e )
        {
            if ( _automaticMessageSender != null )
            {
                _automaticMessageSender.Stop();
            }
            if ( _casino != null )
            {
                _casino.Stop();
            }
			if ( _userManager != null )
			{
				_userManager.Dispose();
			}
			if ( _drinkingGame != null )
			{
				_drinkingGame.Disconnect();
			}
			if ( _connection != null )
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
            if ( _log != null )
            {
                _log.Dispose();
                _log = null;
            }
        }

        private void clearToolStripMenuItem_Click( object sender, EventArgs e )
        {
            textBoxLog.Clear();
        }

        private void imageWindowToolStripMenuItem_Click( object sender, EventArgs e )
        {
            _imageForm.Visible = !imageWindowToolStripMenuItem.Checked;
        }

        private void image_VisibilityChanged( object sender, EventArgs e )
        {
            imageWindowToolStripMenuItem.Checked = ( (Images) sender ).Visible;
        }

        private void textBoxRGB_TextChanged( object sender, EventArgs e )
        {
            bool allValid = true;
            byte r, g, b;

            if ( byte.TryParse( textBoxR.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out r ) )
            {
                textBoxR.BackColor = _windowColor;
            }
            else
            {
                allValid = false;
                textBoxR.BackColor = Color.Red;
            }

            if ( byte.TryParse( textBoxG.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out g ) )
            {
                textBoxG.BackColor = _windowColor;
            }
            else
            {
                allValid = false;
                textBoxG.BackColor = Color.Red;
            }

            if ( byte.TryParse( textBoxB.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out b ) )
            {
                textBoxB.BackColor = _windowColor;
            }
            else
            {
                allValid = false;
                textBoxB.BackColor = Color.Red;
            }

            if ( !allValid )
            {
                return;
            }

            _imageForm.BackColor = Color.FromArgb( r, g, b );
        }

        private void buttonClear_Click( object sender, EventArgs e )
        {
			_raffle.ClearUsers();
            listBoxRaffle.Items.Clear();
        }

        private void buttonDraw_Click( object sender, EventArgs e )
        {
			int drawIndex = _raffle.DrawUserIndex();
			if ( drawIndex >= 0 )
			{
				listBoxRaffle.SelectedIndex = _raffle.DrawUserIndex();
			}
        }

        private void newToolStripMenuItem_Click( object sender, EventArgs e )
        {
            StartLog( false );
        }

        private void appendToolStripMenuItem_Click( object sender, EventArgs e )
        {
            StartLog( true );
        }

        private void logToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( !logToolStripMenuItem.Checked )
            {
                return;
            }

            _log.Dispose();
            _log = null;
            logToolStripMenuItem.Checked = false;
        }

        private void tabControlConnected_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleTabChange( tabControlConnected.SelectedTab );
        }

        private void disconnectToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( _connection == null )
            {
                return;
            }

			ConnectionDisconnected( sender, e );
            HandleConnectionChange( false );
        }

        private void checkBoxPlay_CheckedChanged( object sender, EventArgs e )
        {
            if ( checkBoxPlay.Checked )
            {
				_drinkingGame.StartPlaying( textBoxCharacter1.Text, textBoxCharacter2.Text, textBoxCharacter3.Text, textBoxCharacter4.Text );
            }
            else
            {
				_drinkingGame.StopPlaying();
            }
        }

        private void buttonCharacterDrink_Click( object sender, EventArgs e )
        {
            checkBoxPlay.Checked = true;

            int characterNum;
            if ( sender == buttonCharacter1Drink )
            {
                characterNum = 1;
            }
            else if ( sender == buttonCharacter2Drink )
            {
                characterNum = 2;
            }
            else if ( sender == buttonCharacter3Drink )
            {
                characterNum = 3;
            }
            else if ( sender == buttonCharacter4Drink )
            {
                characterNum = 4;
            }
            else
            {
                return;
            }

			_drinkingGame.GivePlayersDrinks( characterNum );

		}

        private void buttonViewerDrink_Click( object sender, EventArgs e )
        {
            checkBoxPlay.Checked = true;
			_drinkingGame.GivePlayerDrink( comboBoxViewer.Text );
		}

        private void buttonCharacterGetTicket_Click( object sender, EventArgs e )
        {
            checkBoxPlay.Checked = true;

            int characterNum;
            if ( sender == buttonCharacter1GetTicket )
            {
                characterNum = 1;
            }
            else if ( sender == buttonCharacter2GetTicket )
            {
                characterNum = 2;
            }
            else if ( sender == buttonCharacter3GetTicket )
            {
                characterNum = 3;
            }
            else if ( sender == buttonCharacter4GetTicket )
            {
                characterNum = 4;
            }
            else
            {
                return;
            }

			_drinkingGame.GivePlayersTicket( characterNum );
        }

        private void buttonViewerGetTicket_Click( object sender, EventArgs e )
        {
            checkBoxPlay.Checked = true;
			_drinkingGame.GivePlayerTicket( comboBoxViewer.Text );
        }

        private void buttonCharacterFinish_Click( object sender, EventArgs e )
        {
            checkBoxPlay.Checked = true;

            int characterNum;
            if ( sender == buttonCharacter1Finish )
            {
                characterNum = 1;
            }
            else if ( sender == buttonCharacter2Finish )
            {
                characterNum = 2;
            }
            else if ( sender == buttonCharacter3Finish )
            {
                characterNum = 3;
            }
            else if ( sender == buttonCharacter4Finish )
            {
                characterNum = 4;
            }
            else
            {
                return;
            }

			_drinkingGame.PlayersFinishDrinks( characterNum );
        }

        private void buttonViewerFinish_Click( object sender, EventArgs e )
        {
            checkBoxPlay.Checked = true;
			_drinkingGame.PlayerFinishDrink( comboBoxViewer.Text );
        }

        private void buttonAllDrink_Click( object sender, EventArgs e )
        {
            checkBoxPlay.Checked = true;
			_drinkingGame.AllPlayersDrink();
		}

        /// <summary>
        /// Manually add a drinking game participant as an administrative maintenance function.
        /// The new participant does not need to be a viewer, so this can be used to let people
        /// join the game without requiring them to be in the chat.
        /// </summary>
        private void buttonAdd_Click( object sender, EventArgs e )
        {
            string viewer = comboBoxCustom.Text.ToLowerInvariant();
            int characterNum = (int) comboBoxCustomCharacter.SelectedItem;
			_drinkingGame.SetParticipant( viewer, characterNum );

            comboBoxCustom.Text = "";
            comboBoxViewer.Items.Add( viewer );
            comboBoxCustom.Items.Add( viewer );
        }

        /// <summary>
        /// Manually remove a drinking game participant as an administrative maintenance
        /// function. It would be pretty rude to remove someone that is actually a viewer since
        /// they would lose all their drink tickets and they would have to re-join, so
        /// administrators should probably only remove people that they added themselves.
        /// </summary>
        private void buttonRemove_Click( object sender, EventArgs e )
        {
            comboBoxViewer.Items.Remove( comboBoxCustom.Text.ToLowerInvariant() );
            comboBoxCustom.Items.Remove( comboBoxCustom.Text.ToLowerInvariant() );
			_drinkingGame.RemoveParticipant( comboBoxCustom.Text );
        }

        /// <summary>
        /// Update the character number combobox for the manually entered drinking game
        /// participant. This can be used to check the current character assignment of any
        /// participant.
        /// </summary>
        private void comboBoxCustom_SelectedIndexChanged( object sender, EventArgs e )
        {
			string username = (string) comboBoxCustom.SelectedItem;
			if ( _drinkingGame.IsUserPlaying( username ) )
			{
				comboBoxCustomCharacter.SelectedItem = _drinkingGame.GetPlayerNumber( username );
			}
        }

        /// <summary>
        /// Update the participant's character assignment.
        /// </summary>
        private void comboBoxCustomCharacter_SelectedIndexChanged( object sender, EventArgs e )
        {
			_drinkingGame.SetParticipant( comboBoxCustom.Text, (int) comboBoxCustomCharacter.SelectedItem );
        }

        private void ConfigFileButton_Click( object sender, EventArgs e )
        {
            _fileDialog.ShowDialog();
            if ( !string.IsNullOrEmpty( _fileDialog.FileName ) )
            {
				_configFilePath = _fileDialog.FileName;
            }
        }

		private void loadCredentialsButton_Click( object sender, EventArgs e )
		{
			IList<string> credentials = _credentialsReaderWriter.ReadCredentials();
			if ( credentials != null && credentials.Count >= 3 )
			{
				textBoxUser.Text = credentials[ 0 ];
				textBoxPassword.Text = credentials[ 1 ];
				textBoxChat.Text = credentials[ 2 ];
				if ( credentials.Count >= 4 )
				{
					_configFilePath = credentials[ 3 ];
				}
			}
		}
	}
}
