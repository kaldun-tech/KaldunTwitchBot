using System;
using System.Collections.Generic;
using System.IO;

namespace TwitchBot
{
	class UserManager : IDisposable
	{
		/// <summary>
		/// Represents users in the Twitch channel
		/// </summary>
		protected class User
		{
			/// <summary>
			/// Create a user
			/// </summary>
			/// <param name="username">Name of user</param>
			/// <param name="casinoBalance">Starting casino balance</param>
			/// <param name="drinkTickets">Starting number of drink tickets</param>
			/// <param name="drinksTaken">Starting number of drinks taken</param>
			/// <param name="isActive">Whether the user is currently logged into the channel</param>
			public User( string username, uint casinoBalance, uint drinkTickets, uint drinksTaken, bool isActive )
			{
				UserName = username;
				CasinoBalance = casinoBalance;
				DrinkTickets = drinkTickets;
				TotalDrinksTaken = drinksTaken;
				IsActive = isActive;
			}

			public string UserName;
			public uint CasinoBalance;
			public uint DrinkTickets;
			public uint TotalDrinksTaken;
			public bool IsActive;

			public override bool Equals( object obj )
			{
				User casted = obj as User;
				return casted != null && UserName == casted.UserName;
			}

			public override int GetHashCode()
			{
				return UserName == null ? 0 : UserName.GetHashCode();
			}
		}

		/// <summary>
		/// Create a user manager
		/// </summary>
		/// <param name="csvFilePath">Path fo the csv file to write and read data to</param>
		public UserManager( string csvFilePath )
		{
			_csvFilePath = csvFilePath;
			_users = new Dictionary<string, User>();
			ReadFromCsv();
		}

		private bool _disposed = false;
		private object _userLock = new object();
		private Dictionary<string, User> _users;
		private string _csvFilePath;

		/// <summary>
		/// Reload the user manager after it was disconnected
		/// </summary>
		public void Reconnect()
		{
			if ( _disposed )
			{
				_disposed = false;
				ReadFromCsv();
			}
		}

		/// <summary>
		/// Get the list of currently active users
		/// </summary>
		public IList<string> ActiveUsers
		{
			get
			{
				lock ( _userLock )
				{
					List<string> activeUserNames = new List<string>();
					foreach ( KeyValuePair<string, User> userNameUserPair in _users )
					{
						if ( userNameUserPair.Value != null && userNameUserPair.Value.IsActive )
						{
							activeUserNames.Add( userNameUserPair.Key );
						}
					}
					return activeUserNames;
				}
			}
		}

		/// <summary>
		/// Checks whether the user is active
		/// </summary>
		/// <param name="userName"></param>
		public bool IsUserActive( string userName )
		{
			lock ( _userLock )
			{
				User user;
				return _users.TryGetValue( userName, out user ) && user.IsActive;
			}
		}

		/// <summary>
		/// Logs a user into the channel
		/// </summary>
		/// <param name="userName"></param>
		public void LoginUser( string userName )
		{
			lock ( _userLock )
			{
				User user;
				if ( _users.TryGetValue( userName, out user ) )
				{
					user.IsActive = true;
				}
				else
				{
					// Didn't find one, make a new one
					User newUser = new User( userName, 0, 0, 0, true );
					_users.Add( userName, newUser );
				}
			}
		}

		/// <summary>
		/// Logs a user out of the channel
		/// </summary>
		/// <param name="userName"></param>
		public void LogoutUser( string userName )
		{
			lock ( _userLock )
			{
				User user;
				if ( _users.TryGetValue( userName, out user ) )
				{
					user.IsActive = false;
				}
			}
		}

		/// <summary>
		/// Get the casino balance for a user
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public uint GetUserCasinoBalance( string userName )
		{
			lock ( _userLock )
			{
				User user;
				return _users.TryGetValue( userName, out user ) ? user.CasinoBalance : 0 ;
			}
		}

		/// <summary>
		/// Increase the user's currency balance
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="amount"></param>
		public void IncreaseUserCurrency( string userName, uint amount )
		{
			lock ( _userLock )
			{
				User user;
				if ( _users.TryGetValue( userName, out user ) )
				{
					user.CasinoBalance += amount;
				}
			}
		}

		/// <summary>
		/// Decrease the user's currency balance
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="amount"></param>
		public void DecreaseUserCurrency( string userName, uint amount )
		{
			lock ( _userLock )
			{
				User user;
				if ( _users.TryGetValue( userName, out user ) )
				{
					if ( amount > user.CasinoBalance )
					{
						user.CasinoBalance = 0;
					}
					else
					{
						user.CasinoBalance -= amount;
					}
				}
			}
		}

		/// <summary>
		/// Increase all active users' currency balance
		/// </summary>
		/// <param name="amount"></param>
		public void SplashUserCurrency( uint amount )
		{
			lock ( _userLock )
			{
				foreach ( User user in _users.Values )
				{
					if ( user.IsActive )
					{
						user.CasinoBalance += amount;
					}
				}
			}
		}

		/// <summary>
		/// Get the amount of drink tickets that the user has
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public uint GetDrinkTickets( string userName )
		{
			lock ( _userLock )
			{
				User user;
				return _users.TryGetValue( userName, out user ) ? user.DrinkTickets : 0;
			}
		}

		/// <summary>
		/// Increment the number of drink tickets for a single user
		/// </summary>
		/// <param name="userName"></param>
		public void IncrementDrinkTickets( string userName )
		{
			lock ( _userLock )
			{
				User user;
				if ( _users.TryGetValue( userName, out user ) )
				{
					++user.DrinkTickets;
				}
			}
		}

		/// <summary>
		/// Increment the number of drink tickets for a list of users
		/// </summary>
		/// <param name="usernames"></param>
		public void IncrementDrinkTickets( ICollection<string> usernames )
		{
			if ( usernames != null && usernames.Count > 0 )
			{
				lock ( _userLock )
				{
					foreach ( string userName in usernames )
					{
						User user;
						if ( _users.TryGetValue( userName, out user ) )
						{
							++user.DrinkTickets;
						}
					}
				}
			}
		}

		/// <summary>
		/// Give a drink ticket from the source user to the target user
		/// </summary>
		/// <param name="sourceUserName"></param>
		/// <param name="targetUserName"></param>
		public void GiveDrink( string sourceUserName, string targetUserName )
		{
			lock ( _userLock )
			{
				User source, target;
				if ( _users.TryGetValue( sourceUserName, out source ) && _users.TryGetValue( targetUserName, out target ) )
				{
					--source.DrinkTickets;
					++target.TotalDrinksTaken;
				}
			}
		}

		/// <summary>
		/// Get the number of drinks that a user has taken
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public uint GetNumberOfDrinksTaken( string userName )
		{
			lock ( _userLock )
			{
				User user;
				return _users.TryGetValue( userName, out user ) ? user.TotalDrinksTaken : 0;
			}
		}

		/// <summary>
		/// Increment the number of drinks taken for a list of users
		/// </summary>
		/// <param name="usernames"></param>
		public void IncrementDrinksTaken( ICollection<string> usernames )
		{
			if ( usernames != null && usernames.Count > 0 )
			{
				lock ( _userLock )
				{
					foreach ( string userName in usernames )
					{
						User user;
						if ( _users.TryGetValue( userName, out user ) )
						{
							++user.TotalDrinksTaken;
						}
					}
				}
			}
		}

		/// <summary>
		/// Increment the number of drinks taken for a single user
		/// </summary>
		/// <param name="userNames"></param>
		public void IncrementDrinksTaken( string userName )
		{
			lock ( _userLock )
			{
				User user;
				if ( _users.TryGetValue( userName, out user ) )
				{
					++user.TotalDrinksTaken;
				}
			}
		}

		/// <summary>
		/// Write to file and close down
		/// </summary>
		public void Dispose()
		{
			lock ( _userLock )
			{
				if ( !_disposed )
				{
					WriteToCsv();
					_users.Clear();
					_disposed = true;
				}
			}
		}

		/// <summary>
		/// Read users from the csv file if _csvFilePath is set
		/// </summary>
		private void ReadFromCsv()
		{
			if ( string.IsNullOrEmpty( _csvFilePath ) )
			{
				return;
			}

			try
			{
				using ( CsvDataReader reader = new CsvDataReader( _csvFilePath ) )
				{
					string[] dataArray;
					while ( ( dataArray = reader.ReadLine() ) != null )
					{
						if ( dataArray.Length >= 4 )
						{
							string userName = dataArray[ 0 ];
							uint balance = 0;
							uint tickets = 0;
							uint drinksTaken = 0;
							if ( uint.TryParse( dataArray[ 1 ], out balance ) && uint.TryParse( dataArray[ 2 ], out tickets ) && uint.TryParse( dataArray[ 3 ], out drinksTaken ) )
							{
								User user;
								if ( _users.TryGetValue( userName, out user ) )
								{
									user.CasinoBalance = balance;
									user.DrinkTickets = tickets;
									user.TotalDrinksTaken = drinksTaken;
								}
								else
								{
									user = new User( userName, balance, tickets, drinksTaken, false );
									_users.Add( userName, user );
								}
							}
						}
					}
				}
			}
			catch ( IOException ex )
			{
				Console.WriteLine( ex );
			}
		}

		/// <summary>
		/// Writes users to the csv file if _csvFfilePath is set
		/// </summary>
		private void WriteToCsv()
		{
			if ( string.IsNullOrEmpty( _csvFilePath ) )
			{
				return;
			}


			if ( !File.Exists( _csvFilePath ) )
			{
				File.Create( _csvFilePath );
			}

			try
			{
				using ( CsvDataWriter writer = new CsvDataWriter( _csvFilePath ) )
				{
					foreach ( KeyValuePair<string, User> userNameUserPair in _users )
					{
						List<string> dataList = new List<string>();
						dataList.Add( userNameUserPair.Value.UserName );
						dataList.Add( userNameUserPair.Value.CasinoBalance.ToString() );
						dataList.Add( userNameUserPair.Value.DrinkTickets.ToString() );
						dataList.Add( userNameUserPair.Value.TotalDrinksTaken.ToString() );
						writer.WriteLine( dataList );
					}
				}
			}
			catch ( IOException ex )
			{
				Console.WriteLine( ex );
			}
		}
	}
}
