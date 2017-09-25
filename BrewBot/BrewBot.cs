﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using BrewBot.Interfaces;
using BrewBot.Commands;
using BrewBot.Config;
using BrewBot.Connection;
using TwitchLib.Events.Client;

namespace BrewBot
{
	internal partial class BrewBot : Form
    {
		/// <summary>
		/// Create a BrewBot. This does all the UI logic
		/// </summary>
        public BrewBot()
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

			_commandFactory = new CommandFactory( GetCommandsCB, GetBalanceCB, GambleCB, GiveDrinksCB, JoinDrinkingGameCB, QuitDrinkingGameCB, RaffleCB, SplashCurrencyCB, GetDrinkTickets, GetTotalDrinksCB );
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

		// Wait two minutes between sending the commands list
		private readonly TimeSpan TIME_TO_WAIT_BETWEEN_SENDING_COMMANDS_LIST = TimeSpan.FromMinutes( 2 );

		private readonly CommandFactory _commandFactory;
		private readonly LoginCredentialReaderWriter _credentialsReaderWriter;
		private readonly UserManager _userManager;
		private readonly DrinkingGame _drinkingGame;
		private readonly Raffle _raffle;

		private string _chatChannel;
		private TwitchLibConnection _connection;
		private DateTime? _commandsLastSent = null;

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

		private void OnDisconnect( object sender, EventArgs e )
        {
			if ( InvokeRequired )
			{
				BeginInvoke( new EventHandler( OnDisconnect ), sender, e );
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

				_connection.OnDisconnected -= OnDisconnect;
                _connection.OnMessageReceived -= OnMessageReceived;
                _connection.OnMessageSent -= OnMessageSent;
                _connection.OnWhisperReceived -= OnWhisperReceived;
				_connection.OnNewSubscriber -= OnNewSubscriber;
				_connection.OnUserJoined -= OnUserJoined;
                _connection.OnUserLeft -= OnUserLeft;
                _connection = null;
            }

            HandleConnectionChange( false );
        }

		/// <summary>
		/// Used for message transfer
		/// </summary>
		private class MessageTransferArgs : EventArgs
		{
			public MessageTransferArgs( string user, bool isUserMod, string message, bool received )
			{
				UserName = user;
				IsUserModerator = isUserMod;
				Message = message;
				Received = received;
			}

			public MessageTransferArgs( string user, string message, bool received ) : this( user, false, message, received )
			{ }

			public string UserName { get; private set; }
			public bool IsUserModerator { get; private set; }
			public string Message { get; private set; }
			public bool Received { get; private set; }
		}

		private delegate void MessageTransferHandler( object sender, MessageTransferArgs e );

		private void HandleChatTraffic( object sender, MessageTransferArgs e )
		{
			string carrot = e.Received ? " > " : " < ";
			string toAppend = DateTime.Now.ToString( "HH:mm:ss.f" ) + " - " + e.UserName + carrot + e.Message;

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

		private void HandleMessageReceived( object sender, MessageTransferArgs e )
		{
			if ( InvokeRequired )
			{
				BeginInvoke( new MessageTransferHandler( HandleMessageReceived ), sender, e );
				return;
			}

			// We may not have gotten a join notification for this user yet.
			if ( !_userManager.IsUserActive( e.UserName ) )
			{
				_userManager.LoginUser( e.UserName );
			}

			ICommand command = _commandFactory.CreateCommand( e.Message, e.UserName, e.IsUserModerator );
			if ( command != null )
			{
				command.ExecuteCommand();
			}

			_drinkingGame.IntroduceUser( e.UserName, textBoxCharacter1.Text, textBoxCharacter2.Text, textBoxCharacter3.Text, textBoxCharacter4.Text );
			HandleChatTraffic( sender, e );
		}

		private void HandleMessageSent( object sender, MessageTransferArgs e )
		{
			if ( InvokeRequired )
			{
				BeginInvoke( new MessageTransferHandler( HandleMessageSent ), sender, e );
				return;
			}

			HandleChatTraffic( sender, e );
		}

		private void OnMessageReceived( object sender, OnMessageReceivedArgs e )
        {
			// TODO configure a list of banned words
			if ( e.ChatMessage.Message.Contains( "badword" ) )
			{
				// TODO time user out
			}

			// Broadcaster also has moderator privileges
			bool isBroadcasterOrModerator = e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster;
			HandleMessageReceived( sender, new MessageTransferArgs( e.ChatMessage.Username, isBroadcasterOrModerator, e.ChatMessage.Message, true ) );
        }

		private void OnWhisperReceived( object sender, OnWhisperReceivedArgs e )
		{
			// Whispers will not be treated with moderator privileges
			HandleMessageReceived( sender, new MessageTransferArgs( e.WhisperMessage.Username, e.WhisperMessage.Message, true ) );
		}

		private void OnMessageSent( object sender, OnMessageSentArgs e )
		{
			// Sent messages will not be treated with moderator privileges
			HandleMessageSent( sender, new MessageTransferArgs( e.SentMessage.DisplayName, e.SentMessage.Message, false ) );
		}

		private void OnNewSubscriber( object sender, OnNewSubscriberArgs e )
		{
			string format = e.Subscriber.IsTwitchPrime ? Strings.SubscriptionReceivedPrime : Strings.SubscriptionReceived;
			string message = string.Format( format, e.Subscriber.DisplayName, _configReader.SubscriberTitle );
			_connection.Send( message );
		}

		private void OnUserJoined( object sender, OnUserJoinedArgs e )
		{
			_userManager.LoginUser( e.Username );
		}

		private delegate void OnUserLeftEvent( object sender, OnUserLeftArgs e );

		private void OnUserLeft( object sender, OnUserLeftArgs e )
		{
			if ( InvokeRequired )
			{
				BeginInvoke( new OnUserLeftEvent( OnUserLeft ), sender, e );
				return;
			}

			_userManager.LogoutUser( e.Username );

			comboBoxViewer.Items.Remove( e.Username.ToLowerInvariant() );
			comboBoxCustom.Items.Remove( e.Username.ToLowerInvariant() );
			_drinkingGame.RemoveParticipant( e.Username );
		}

		private void GetCommandsCB( string sender, string target )
		{
			DateTime currentTime = DateTime.UtcNow;
			// Send if we have waited a proper amount of time between the last send
			if ( !_commandsLastSent.HasValue || currentTime.Subtract( _commandsLastSent.Value ) > TIME_TO_WAIT_BETWEEN_SENDING_COMMANDS_LIST )
			{
				List<string> commandDescriptionList = ( _commandFactory == null ) ? new List<string>() : _commandFactory.GetCommandDescriptionList();
				foreach ( string description in commandDescriptionList )
				{
					_connection.Send( description );
				}
				_commandsLastSent = currentTime;
			}
			else
			{
				_connection.SendWhisper( sender, Strings.CommandOnCooldown );
			}
		}

        private void GetBalanceCB( string sender, string target )
        {
            string message = ( _casino == null ) ? "The casino is not currently operating, kupo!" : _casino.GetStringBalance( sender );
			_connection.Send( message );
        }

        private void GambleCB( string sender, string target )
        {
            string message = null;
            if ( _casino == null )
            {
                message = string.Format( Strings.Casino_NotOperating, sender );
            }
            else
            {
                // Target is the gamble amount
                int betAmount = 0;
                if ( int.TryParse( target, out betAmount ) && betAmount > 0 )
                {
					message = _casino.Gamble( sender, (uint) betAmount );
				}
                else
                {
                    message = string.Format( Strings.Casino_InvalidBetAmount, sender );
                }
            }
            _connection.Send( message );
        }

        private void GiveDrinksCB( string sender, string target )
        {
			_drinkingGame.GivePlayerDrink( sender, target );
			_drinkingGame.AddIntroducedUser( sender );
        }

        private void JoinDrinkingGameCB( string sender, string target )
        {
            DrinkingAdd( sender, target );
			_drinkingGame.AddIntroducedUser( sender );
        }

        private void QuitDrinkingGameCB( string sender, string target )
        {
            string fromToLower = sender.ToLowerInvariant();
            comboBoxViewer.Items.Remove( fromToLower );
            comboBoxCustom.Items.Remove( fromToLower );
			_drinkingGame.RemoveParticipant( sender );
			_drinkingGame.AddIntroducedUser( sender );
        }

        private void RaffleCB( string sender, string target )
        {
            RaffleAdd( sender );
        }

        // TODO: We need the concept of an admin so randos can't just be splashing
        private void SplashCurrencyCB( string sender, string target )
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

		private void GetDrinkTickets( string sender, string target )
		{
			uint drinkTickets = _userManager.GetDrinkTickets( sender );
			string message = string.Format( Strings.DrinkTicketsBalance, sender, drinkTickets );
			_connection.Send( message );
		}

		private void GetTotalDrinksCB( string sender, string target )
		{
			uint numberOfDrinks = _userManager.GetNumberOfDrinksTaken( sender );
			string message = string.Format( Strings.TotalDrinksTaken, sender, numberOfDrinks );
			_connection.Send( message );
		}

        private void buttonDoWork_Click( object sender, EventArgs e )
        {
            if ( _connection != null )
            {
				_drinkingGame.Disconnect();
                _connection.OnDisconnected -= OnDisconnect;
                _connection.OnMessageReceived -= OnMessageReceived;
				_connection.OnMessageSent -= OnMessageSent;
                _connection.OnWhisperReceived -= OnWhisperReceived;
				_connection.OnNewSubscriber -= OnNewSubscriber;
                _connection.OnUserJoined -= OnUserJoined;
                _connection.OnUserLeft -= OnUserLeft;
            }
			
			string username = textBoxUser.Text;
			string oauth = textBoxPassword.Text;
			_chatChannel = textBoxChat.Text;
			_configReader = new ConfigurationReader( _configFilePath );

			_connection = new TwitchLibConnection( _chatChannel, username, oauth );
            _connection.OnDisconnected += OnDisconnect;
            _connection.OnMessageReceived += OnMessageReceived;
            _connection.OnMessageSent += OnMessageSent;
            _connection.OnWhisperReceived += OnWhisperReceived;
			_connection.OnNewSubscriber += OnNewSubscriber;
            _connection.OnUserJoined += OnUserJoined;
            _connection.OnUserLeft += OnUserLeft;
            _connection.Connect();
			_drinkingGame.Connect( _connection );

			if ( saveCredentialsCheckbox.Checked )
			{
				_credentialsReaderWriter.WriteCredentials( username, oauth, _chatChannel, _configFilePath );
			}

            // Configure the automatic message sender thread
            if ( _configReader != null )
            {
                _automaticMessageSender = new ConfigurableMessageSender( _connection, _configReader.ConfiguredMessageIntervalInSeconds, _configReader.GetConfiguredMessages );
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
        private void BrewBot_FormClosed( object sender, FormClosedEventArgs e )
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
                _connection.OnDisconnected -= OnDisconnect;
                _connection.OnMessageReceived -= OnMessageReceived;
                _connection.OnMessageSent -= OnMessageSent;
                _connection.OnWhisperReceived -= OnWhisperReceived;
				_connection.OnNewSubscriber -= OnNewSubscriber;
                _connection.OnUserJoined -= OnUserJoined;
                _connection.OnUserLeft -= OnUserLeft;
                _connection.Disconnect();
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

			_connection.Disconnect();
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