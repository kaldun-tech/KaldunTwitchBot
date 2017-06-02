using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace TwitchBot
{
    class Casino
    {
        // The sleeptime is one minute
        private const int SLEEPTIME_MILLIS = 60 * 1000;

		private readonly string _csvFilePath;
		private readonly string _currencyName;
		private List<CasinoUser> _users;
        private uint _earnedCurrencyPerMinute;
        private double _winChance;
        private object _lock;
        private Thread _earningsThread;

		/// <summary>
		/// Represents players for the casino. These are users in chat who may or may not be currently active
		/// </summary>
		internal class CasinoUser
		{
			/// <summary>
			/// Default constructor. Creates an empty CasinoUser
			/// </summary>
			public CasinoUser()
			{ }

			/// <summary>
			/// Create a casino user given a name, balance, and whether the user is currently active
			/// </summary>
			/// <param name="username"></param>
			/// <param name="balance"></param>
			/// <param name="isActive"></param>
			public CasinoUser( string username, uint balance, bool isActive )
			{
				UserName = username;
				Balance = balance;
				IsActive = isActive;
			}

			public string UserName;
			public uint Balance;
			public bool IsActive;

			public override bool Equals( object obj )
			{
				CasinoUser casted = obj as CasinoUser;
				return casted != null && UserName == casted.UserName;
			}

			public override int GetHashCode()
			{
				return UserName == null ? 0 : UserName.GetHashCode();
			}

			public override string ToString()
			{
				return string.Format( "{0},{1}", UserName, Balance );
			}
		}

		/// <summary>
		/// Create a basic casino
		/// </summary>
		/// <param name="currencyName">Name of the currency. Must not be null or empty.</param>
		/// <param name="currentUsers">List of usernames on casino startup.</param>
		/// <param name="earnRate">Amount of currency earned per minute. Must be positive.</param>
		/// <param name="winChance">Chance to win when gambling. Must be greater than zero and less than or equal to one.</param>
        public Casino( string currencyName, List<string> currentUsers, uint earnRate, double winChance ) : this( null, currencyName, currentUsers, earnRate, winChance)
        { }

		/// <summary>
		/// Create a casino that will read and write to a csv file
		/// </summary>
		/// <param name="csvFilePath">File path to the csv file that contains user data.</param>
		/// <param name="currencyName">Name of the currency. Must not be null or empty.</param>
		/// <param name="currentUsers">List of usernames on casino startup.</param>
		/// <param name="earnRate">Amount of currency earned per minute. Must be positive.</param>
		/// <param name="winChance">Chance to win when gambling. Must be greater than zero and less than or equal to one.</param>
		public Casino( string csvFilePath, string currencyName, List<string> currentUsers, uint earnRate, double winChance )
		{
			_csvFilePath = csvFilePath;
			_currencyName = currencyName;
			_users = new List<CasinoUser>( currentUsers.Count );
			_earnedCurrencyPerMinute = earnRate;
			_winChance = winChance;
			_lock = new object();
			_earningsThread = new Thread( new ThreadStart( AccrueEarnings ) );

			if ( currentUsers != null && currentUsers.Count > 0)
			{
				foreach ( string username in currentUsers )
				{
					CasinoUser user = new CasinoUser( username, 0, true );
					_users.Add( user );
				}
			}
		}

		/// <summary>
		/// Name of the currency
		/// </summary>
		public string CurrencyName
        {
            get { return _currencyName; }
        }

		/// <summary>
		/// Logs a user into the casino
		/// </summary>
		/// <param name="userName">Name of user. Should not be null or empty.</param>
        public void LoginUser( string userName )
        {
            lock ( _lock )
            {
				CasinoUser found = FindUser( userName );
				if ( found == null )
				{
					// Didn't find one, make a new one
					CasinoUser newUser = new CasinoUser( userName, 0, true );
					_users.Add( newUser );
				}
				else
				{
					found.IsActive = true;
				}
			}
        }

		/// <summary>
		/// Logs a user out of the casino
		/// </summary>
		/// <param name="userName">Name of user. Should not be null or empty.</param>
		public void LogoutUser( string userName )
        {
            lock ( _lock )
            {
				CasinoUser found = FindUser( userName );
				if ( found != null )
				{
					found.IsActive = false;
				}
            }
        }

		/// <summary>
		/// Splash all active users with an amount of currency
		/// </summary>
		/// <param name="splashAmount">Amount of currency to splash with</param>
        public void SplashUsers( uint splashAmount )
        {
            lock ( _lock )
            {
				foreach ( CasinoUser user in _users )
				{
					if ( user.IsActive )
					{
						user.Balance += splashAmount;
					}
				}
            }
        }

		/// <summary>
		/// Gamble currency for a user. The user may lose or gain currency.
		/// </summary>
		/// <param name="username">Name of user. Must not be null or empty.</param>
		/// <param name="betAmount">Amount to bet on the gamble. Must be positive and less than or equal to the user's balance.</param>
		/// <returns>The amount of winnings. This will be negative on a loss and zero on error.</returns>
        public long Gamble( string username, uint betAmount )
        {
			CasinoUser found = FindUser( username );
			if ( found != null )
			{
				Random rand = new Random();
				double result = rand.NextDouble();
				if ( result >= _winChance )
				{
					found.Balance += betAmount;
					return betAmount;
				}
				else
				{
					found.Balance -= betAmount;
					return -1 * betAmount;
				}
			}
			return 0;
        }

		/// <summary>
		/// Can the user gamble the given amount
		/// </summary>
		/// <param name="username">Name of user. Must not be null or empty</param>
		/// <param name="betAmount">Amount to attempt to gamble</param>
		/// <returns>Whether the input user can gamble the input amount</returns>
        public bool CanUserGamble( string username, uint betAmount )
        {
			CasinoUser found = FindUser( username );
			return CanUserGamble( found, betAmount );

		}

		/// <summary>
		/// Can the user gamble the given amount
		/// </summary>
		/// <param name="user">User object to try to gamble with. Must not be null</param>
		/// <param name="betAmount">Amount to attempt to gamble</param>
		/// <returns>Whether the input user can gamble the input amount</returns>
		public bool CanUserGamble( CasinoUser user, uint betAmount )
		{
			return user != null && user.Balance >= betAmount;
		}

		/// <summary>
		/// Get the casino balance for the user
		/// </summary>
		/// <param name="username">Name of user. Must not be null or empty.</param>
		/// <returns>The balance for the user. Zero if not found</returns>
		public uint GetBalance( string username )
        {
            lock ( _lock )
            {
				CasinoUser found = FindUser( username );
				return found == null ? 0 : found.Balance;
            }
        }

		/// <summary>
		/// Start the earnings thread
		/// </summary>
        public void Start()
		{
			ReadFromCsv();
			if ( !_earningsThread.IsAlive )
            {
                _earningsThread.Start();
            }
        }

		/// <summary>
		/// Stop the earnings thread
		/// </summary>
        public void Stop()
        {
            _earningsThread.Abort();
			WriteToCsv();
		}

		/// <summary>
		/// Find a user for a username
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		private CasinoUser FindUser( string userName )
		{
			return _users.Find( x => x.UserName == userName );
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
				if ( !File.Exists( _csvFilePath ) )
				{
					File.Create( _csvFilePath );
				}

				using ( StreamReader reader = new StreamReader( _csvFilePath ) )
				{
					string line;
					while ( !string.IsNullOrEmpty( line = reader.ReadLine() ) )
					{
						//Define pattern
						string[] split = line.Split( new char[] { ',' } );
						if ( split != null && split.Length >= 2 )
						{
							string username = split[ 0 ];
							uint balance;
							if ( uint.TryParse( split[ 1 ], out balance ) )
							{
								CasinoUser user;
								if ( ( user = FindUser( username ) ) == null )
								{
									user = new CasinoUser( username, balance, false );
									_users.Add( user );
								}
								else
								{
									user.Balance = balance;
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
		/// Write users to the csv file if _csvFilePath is set
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

			StringBuilder builder = new StringBuilder();
			foreach ( CasinoUser user in _users )
			{
				builder.AppendLine( user.ToString() );
			}
			string result = builder.ToString();

			try
			{
				using ( StreamWriter writer = new StreamWriter( _csvFilePath ) )
				{
					writer.Write( result );
				}
			}
			catch ( IOException ex )
			{
				Console.WriteLine( ex );
			}
		}

		/// <summary>
		/// Earnings thread accrues currency for all active users
		/// </summary>
		private void AccrueEarnings()
        {
            try
            {
                while ( true )
                {
                    SplashUsers( _earnedCurrencyPerMinute );
                    Thread.Sleep( SLEEPTIME_MILLIS );
                }
            }
            catch ( ThreadInterruptedException )
            { }
        }
    }
}
