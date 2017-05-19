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

		internal class CasinoUser
		{
			public CasinoUser()
			{ }

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

        public Casino( string currencyName, List<string> currentUsers, uint earnRate, double winChance ) : this( null, currencyName, currentUsers, earnRate, winChance)
        { }

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

		public string CurrencyName
        {
            get { return _currencyName; }
        }

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

        public bool CanUserGamble( string username, uint betAmount )
        {
			CasinoUser found = FindUser( username );
			return CanUserGamble( found, betAmount );

		}

		public bool CanUserGamble( CasinoUser user, uint betAmount )
		{
			return user != null && user.Balance >= betAmount;
		}

		public uint GetBalance( string username )
        {
            lock ( _lock )
            {
				CasinoUser found = FindUser( username );
				return found == null ? 0 : found.Balance;
            }
        }

        public void Start()
		{
			ReadFromCsv();
			if ( !_earningsThread.IsAlive )
            {
                _earningsThread.Start();
            }
        }

        public void Stop()
        {
            _earningsThread.Abort();
			WriteToCsv();
		}

		private CasinoUser FindUser( string userName )
		{
			return _users.Find( x => x.UserName == userName );
		}

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
