using System;
using System.Collections.Generic;
using System.Text;

namespace BrewBot
{
	public class DrinkingGame
	{
		/// <summary>
		/// Create a drinking game
		/// </summary>
		/// <param name="userManager"></param>
		public DrinkingGame( UserManager userManager )
		{
			_userManager = userManager;
			_drinkingGameParticipants = new Dictionary<string, int>( StringComparer.InvariantCultureIgnoreCase );
			_introducedUsers = new HashSet<string>();
		}

		private UserManager _userManager;
		private Connection _connection;
		// Maps case insensitive usernames to player numbers
		private IDictionary<string, int> _drinkingGameParticipants;
		// Users who have seen the drinking game intro
		private HashSet<string> _introducedUsers;

		/// <summary>
		/// Are we playing the drinking game?
		/// </summary>
		public bool IsPlaying
		{
			get; private set;
		}

		/// <summary>
		/// Start playing the game with the input players
		/// </summary>
		/// <param name="player1"></param>
		/// <param name="player2"></param>
		/// <param name="player3"></param>
		/// <param name="player4"></param>
		public void StartPlaying( string player1, string player2, string player3, string player4 )
		{
			if ( !IsPlaying )
			{
				IsPlaying = true;
				SendToConnection( string.Format( Strings.DrinkingGame_Start,
						player1, player2, player3, player4 ) );
				IntroduceAllActiveUsers();
			}
		}

		/// <summary>
		/// Stop playing the drinking game
		/// </summary>
		public void StopPlaying()
		{
			if ( IsPlaying )
			{
				IsPlaying = false;
				SendToConnection( "The drinking game has ended." );
				_drinkingGameParticipants.Clear();
				_introducedUsers.Clear();
			}
		}

		/// <summary>
		/// Is the user playing the drinking game?
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public bool IsUserPlaying( string username )
		{
			return _drinkingGameParticipants.ContainsKey( username );
		}

		/// <summary>
		/// Get the player number for a user
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public int GetPlayerNumber( string username )
		{
			return IsUserPlaying( username ) ? _drinkingGameParticipants[ username ] : 0;
		}

		/// <summary>
		/// Add or update a drinking game participant
		/// </summary>
		/// <param name="username"></param>
		/// <param name="playerNumber"></param>
		public void SetParticipant( string username, int playerNumber )
		{
			if ( IsUserPlaying( username ) )
			{
				_drinkingGameParticipants[ username ] = playerNumber;
			}
			else
			{
				_drinkingGameParticipants.Add( username, playerNumber );
			}
		}

		/// <summary>
		/// Remove a drinking game participant
		/// </summary>
		/// <param name="username"></param>
		public void RemoveParticipant( string username )
		{
			if ( IsUserPlaying( username ) )
			{
				_drinkingGameParticipants.Remove( username );
			}
		}

		/// <summary>
		/// Give a player a drink
		/// </summary>
		/// <param name="username"></param>
		public void GivePlayerDrink( string username )
		{
			if ( !IsPlaying )
			{
				SendToConnection( string.Format( Strings.NoDrinkingGame, username ) );
				return;
			}

			if ( IsUserPlaying( username ) )
			{
				_userManager.IncrementDrinksTaken( username );
				SendToConnection( string.Format( Strings.TakeDrink, username ) );
			}
		}

		/// <summary>
		/// Give all players mapped to a given player number drinks
		/// </summary>
		/// <param name="playerNumber"></param>
		public void GivePlayersDrinks( int playerNumber )
		{
			if ( !IsPlaying )
			{
				return;
			}

			StringBuilder messageTargets = new StringBuilder();
			foreach ( KeyValuePair<string, int> viewerAssignment in _drinkingGameParticipants )
			{
				if ( viewerAssignment.Value == playerNumber )
				{
					string username = viewerAssignment.Key;
					_userManager.IncrementDrinksTaken( username );
					if ( messageTargets.Length > 0 )
					{
						messageTargets.Append( ", @" );
					}
					messageTargets.Append( username );
				}
			}

			if ( messageTargets.Length > 0 )
			{
				SendToConnection( string.Format( Strings.TakeDrink, messageTargets ) );
			}
		}

		/// <summary>
		/// A source user gives another user a drink by using a drink ticket
		/// </summary>
		/// <param name="sourceUser"></param>
		/// <param name="targetUser"></param>
		public void GivePlayerDrink( string sourceUser, string targetUser )
		{
			if ( !IsPlaying )
			{
				SendToConnection( string.Format( Strings.NoDrinkingGame, sourceUser ) );
				return;
			}

			if ( !IsUserPlaying( targetUser ) )
			{
				SendToConnection( string.Format( Strings.NotParticipating, sourceUser, targetUser ) );
				return;
			}
			
			if ( _userManager.GetDrinkTickets( sourceUser ) == 0 )
			{
				SendToConnection( string.Format( Strings.NoDrinkTickets, sourceUser ) );
				return;
			}

			_userManager.GiveDrink( sourceUser, targetUser );
			SendToConnection( string.Format( Strings.TakeDrink, targetUser ) );
		}

		/// <summary>
		/// All players drink!
		/// </summary>
		public void AllPlayersDrink()
		{
			if ( !IsPlaying )
			{
				return;
			}

			_userManager.IncrementDrinksTaken( _drinkingGameParticipants.Keys );
			SendToConnection( "Everyone drink!" );
		}

		/// <summary>
		/// Tell a player to finish their drink
		/// </summary>
		/// <param name="username"></param>
		public void PlayerFinishDrink( string username )
		{
			if ( !IsPlaying )
			{
				SendToConnection( string.Format( Strings.NoDrinkingGame, username ) );
				return;
			}

			if ( IsUserPlaying( username ) )
			{
				_userManager.IncrementDrinksTaken( username );
				SendToConnection( string.Format( Strings.FinishDrink, username ) );
			}
		}

		/// <summary>
		/// Tell players mapped to a given player number to finish their drinks
		/// </summary>
		/// <param name="playerNumber"></param>
		public void PlayersFinishDrinks( int playerNumber)
		{
			if ( !IsPlaying )
			{
				return;
			}

			StringBuilder messageTargets = new StringBuilder();
			foreach ( KeyValuePair<string, int> viewerAssignment in _drinkingGameParticipants )
			{
				string username = viewerAssignment.Key;
				_userManager.IncrementDrinksTaken( username );
				if ( viewerAssignment.Value == playerNumber )
				{
					if ( messageTargets.Length > 0 )
					{
						messageTargets.Append( ", @" );
					}
					messageTargets.Append( username );
				}
			}

			if ( messageTargets.Length > 0 )
			{
				SendToConnection( string.Format( Strings.FinishDrink, messageTargets ) );
			}
		}

		/// <summary>
		/// Give a single user a drink ticket
		/// </summary>
		/// <param name="username"></param>
		public void GivePlayerTicket( string username )
		{
			if ( !IsPlaying )
			{
				SendToConnection( string.Format( Strings.NoDrinkingGame, username ) );
				return;
			}

			if ( IsUserPlaying( username ) )
			{
				_userManager.IncrementDrinkTickets( username );
				SendToConnection( string.Format( Strings.GetDrinkTicket, username, _userManager.GetDrinkTickets( username ) ) );
			}
		}

		/// <summary>
		/// Give players a drink ticket who are mapped to a given player number
		/// </summary>
		/// <param name="playerNumber"></param>
		public void GivePlayersTicket( int playerNumber )
		{
			if ( !IsPlaying )
			{
				return;
			}

			StringBuilder messageTargets = new StringBuilder();
			foreach ( KeyValuePair<string, int> viewerAssignment in _drinkingGameParticipants )
			{
				if ( viewerAssignment.Value == playerNumber )
				{
					string username = viewerAssignment.Key;
					_userManager.IncrementDrinkTickets( username );
					if ( messageTargets.Length > 0 )
					{
						messageTargets.Append( ", @" );
					}
					messageTargets.Append( username );
				}
			}

			if ( messageTargets.Length > 0 )
			{
				SendToConnection( string.Format( Strings.GetDrinkTicket, messageTargets ) );
			}
		}

		/// <summary>
		/// Introduce a user to the drinking game being played
		/// </summary>
		/// <param name="username"></param>
		/// <param name="player1"></param>
		/// <param name="player2"></param>
		/// <param name="player3"></param>
		/// <param name="player4"></param>
		public void IntroduceUser( string username, string player1, string player2, string player3, string player4 )
		{
			if ( IsPlaying && !_introducedUsers.Contains( username ) )
			{
				SendToConnection( string.Format( Strings.DrinkingGame_Introduction,
					player1, player2, player3, player4 ) );
				_introducedUsers.Add( username );
				IntroduceAllActiveUsers();				
			}
		}

		/// <summary>
		/// Introduce all active users to our drinking game
		/// </summary>
		private void IntroduceAllActiveUsers()
		{
			foreach ( string nextActiveUser in _userManager.ActiveUsers )
			{
				if ( !_introducedUsers.Contains( nextActiveUser ) )
				{
					_introducedUsers.Add( nextActiveUser );
				}
			}
		}

		/// <summary>
		/// Add a user that does not need to be introduced to the drinking game
		/// </summary>
		/// <param name="username"></param>
		public void AddIntroducedUser( string username )
		{
			if ( !_introducedUsers.Contains( username ) )
			{
				_introducedUsers.Add( username );
			}
		}

		public bool IsConnected
		{
			get { return _connection != null; }
		}

		/// <summary>
		/// Connect the drinking game. Necessary to send messages
		/// </summary>
		/// <param name="connection"></param>
		internal void Connect( Connection connection )
		{
			_connection = connection;
		}

		/// <summary>
		/// Disconnection the connection
		/// </summary>
		public void Disconnect()
		{
			_connection = null;
		}

		/// <summary>
		/// Sends a message to the connection if we are connected
		/// </summary>
		/// <param name="message"></param>
		public void SendToConnection( string message )
		{
			if ( IsConnected )
			{
				_connection.Send( message );
			}
		}
	}
}
