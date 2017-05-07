using System;
using System.Collections.Generic;
using System.Threading;

namespace TwitchBot
{
    class Casino
    {
        // The sleeptime is one minute
        private const int SLEEPTIME_MILLIS = 60 * 1000;
        private readonly string _currencyName;
        private Dictionary<string, int> _userBalances;
        private int _earnedCurrencyPerMinute;
        private double _winChance;
        private object _lock;
        private Thread _earningsThread;

        public Casino( string currencyName, List<string> userNames, int earnRate, double winChance )
        {
            _currencyName = currencyName;
            _userBalances = new Dictionary<string, int>( userNames.Count );
            _earnedCurrencyPerMinute = earnRate;
            _winChance = winChance;
            _lock = new object();
            _earningsThread = new Thread( new ThreadStart( AccrueEarnings ) );
            foreach ( string username in userNames )
            {
                _userBalances.Add( username, 0 );
            }
        }

        public string CurrencyName
        {
            get { return _currencyName; }
        }

        public void LoginUser( string userName, int startingCurrency )
        {
            lock ( _lock )
            {
                if ( !_userBalances.ContainsKey( userName ) )
                {
                    _userBalances.Add( userName, startingCurrency );
                }
            }
        }

        // TODO: Get the starting currency from the database
        public void LoginUser( string userName )
        {
            LoginUser( userName, 0 );
        }

        public void LogoutUser( string userName )
        {
            lock ( _lock )
            {
                if ( _userBalances.ContainsKey( userName ) )
                {
                    //todo write to database
                    _userBalances.Remove( userName );
                }
            }
        }

        public void SplashUsers( int splashAmount )
        {
            lock ( _lock )
            {
                Dictionary<string, int> balancesCopy = new Dictionary<string, int>( _userBalances );
                _userBalances.Clear();
                foreach ( KeyValuePair<string, int> balance in balancesCopy )
                {
                    _userBalances.Add( balance.Key, balance.Value + splashAmount );
                }
            }
        }

        private void IncrementBalance( string username, int amount )
        {
            lock ( _lock )
            {
                _userBalances[ username ] += amount;
            }
        }

        public int Gamble( string username, int betAmount )
        {
            if ( CanUserGamble( username, betAmount ) )
            {
                Random rand = new Random();
                double result = rand.NextDouble();
                int winnings = ( result >= _winChance ) ? betAmount : -1 * betAmount;
                IncrementBalance( username, winnings );
                return winnings;
            }
            return 0;
        }

        public bool CanUserGamble( string username, int betAmount )
        {
            return GetBalance( username ) >= betAmount;
        }

        public int GetBalance( string username )
        {
            lock ( _lock )
            {
                int balance = 0;
                _userBalances.TryGetValue( username, out balance );
                return balance;
            }
        }

        public void Start()
        {
            if ( !_earningsThread.IsAlive && _earnedCurrencyPerMinute <= 0 )
            {
                _earningsThread.Start();
            }
        }

        public void Stop()
        {
            _earningsThread.Abort();
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
