using System;
using System.Threading;

namespace BrewBot
{
	public class Casino
    {
		public enum CanGambleResult
		{
			CAN_GAMBLE,
			BELOW_MINIMUM_BET,
			INSUFFICIENT_FUNDS,
			ON_COOLDOWN
		}

		/// <summary>
		/// Represents players for the casino. These are users in chat who may or may not be currently active
		/// </summary>
		internal class CasinoUser
		{
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
		}

		/// <summary>
		/// Create a casino that will read and write to a csv file
		/// </summary>
		/// <param name="userManager">The user manager for the channel</param>
		/// <param name="currencyName">Name of the currency. Must not be null or empty.</param>
		/// <param name="earnRate">Amount of currency earned per minute.</param>
		/// <param name="minimumGambleAmount">Minimum amount of currency to gamble</param>
		/// <param name="minimumGambleAmountSeconds"></param>
		/// <param name="winChance">Chance to win when gambling. Must be greater than zero and less than or equal to one.</param>
		public Casino( UserManager userManager, string currencyName, uint earnRate, uint minimumGambleAmount, uint minimumGambleAmountSeconds, double winChance )
		{
			_userManager = userManager;
			_currencyName = currencyName;
			_minimumGambleAmount = minimumGambleAmount;
			_minimumGambleIntervalSeconds = minimumGambleAmountSeconds;
			_earnedCurrencyPerMinute = earnRate;
			_winChance = winChance;
			_lock = new object();
			_earningsThread = new Thread( new ThreadStart( AccrueEarnings ) );
		}

		// The sleeptime is one minute
		private const int SLEEPTIME_MILLIS = 60 * 1000;
		
		private readonly string _currencyName;
		private UserManager _userManager;
		private uint _earnedCurrencyPerMinute;
		private uint _minimumGambleAmount;
		private uint _minimumGambleIntervalSeconds;
		private double _winChance;
		private object _lock;
		private Thread _earningsThread;

		/// <summary>
		/// Name of the currency
		/// </summary>
		public string CurrencyName
        {
            get { return _currencyName; }
        }

		/// <summary>
		/// Splash all active users with an amount of currency
		/// </summary>
		/// <param name="splashAmount">Amount of currency to splash with</param>
		/// <returns>Whether the splash attempt was successful</returns>
        public bool SplashUsers( uint splashAmount )
        {
			if ( splashAmount > 0 )
			{
				lock ( _lock )
				{
					_userManager.SplashUserCurrency( splashAmount );
					return true;
				}
			}
			return false;
        }

		/// <summary>
		/// Gamble currency for a user. The user may lose or gain currency. Return a string reprentation of the results.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="betAmount"></param>
		/// <returns>String represenation of the gamble result</returns>
		public string Gamble( string username, uint betAmount )
		{
			string message = null;
			switch( CanUserGamble( username, betAmount ) )
			{
				case CanGambleResult.CAN_GAMBLE:
					long winnings = DoGamble( username, betAmount );
					string winLoseString = winnings > 0 ? "won" : "lost";
					message = string.Format( Strings.Gamble_Result, username, winLoseString, Math.Abs( winnings ), CurrencyName );
					break;
				case CanGambleResult.INSUFFICIENT_FUNDS:
					message = string.Format( Strings.Gamble_InsufficentFunds, username );
					break;
				case CanGambleResult.BELOW_MINIMUM_BET:
					message = string.Format( Strings.Gamble_BelowMinimumBet, _minimumGambleAmount, username);
					break;
				case CanGambleResult.ON_COOLDOWN:
					message = string.Format( Strings.Gamble_OnCooldown, username, _minimumGambleIntervalSeconds );
					break;
			}
			return message;
		}

		/// <summary>
		/// Gamble currency for a user. The user may lose or gain currency.
		/// </summary>
		/// <param name="username">Name of user. Must not be null or empty.</param>
		/// <param name="betAmount">Amount to bet on the gamble. Must be positive and less than or equal to the user's balance.</param>
		/// <returns>The amount of winnings. This will be negative on a loss and zero on error.</returns>
        private long DoGamble( string username, uint betAmount )
        {
			if ( CanGambleResult.CAN_GAMBLE.Equals( CanUserGamble( username, betAmount )) )
			{
				Random rand = new Random();
				double result = rand.NextDouble();

				// Set the gamble time to current time
				_userManager.SetUserLastGambleTime( username );

				if ( result <= _winChance )
				{
					_userManager.IncreaseUserCurrency( username, betAmount );
					return betAmount;
				}
				else
				{
					_userManager.DecreaseUserCurrency( username, betAmount );
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
        public CanGambleResult CanUserGamble( string username, uint betAmount )
        {
			// Calculate whether gamble is on cooldown for this user
			DateTime? lastGambleTime = _userManager.GetUserLastGambleTime( username );
			
			if ( lastGambleTime != null )
			{
				DateTime gambleOffCooldown = lastGambleTime.Value.AddSeconds( _minimumGambleIntervalSeconds );
				// We are still on cooldown if the time when we come off cooldown is after right now
				if ( gambleOffCooldown.CompareTo( DateTime.Now ) > 0 )
				{
					return CanGambleResult.ON_COOLDOWN;
				}
			}
			if ( betAmount < _minimumGambleAmount)
			{
				return CanGambleResult.BELOW_MINIMUM_BET;
			}
			if ( _userManager.GetUserCasinoBalance( username ) < betAmount )
			{
				return CanGambleResult.INSUFFICIENT_FUNDS;
			}
			

			return CanGambleResult.CAN_GAMBLE;
		}

		/// <summary>
		/// Get the string representation of the casino balance for the user
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public string GetStringBalance( string username )
		{
			uint balance = GetCurrencyBalance( username );
			return string.Format( Strings.GetBalance_Result, username, balance, CurrencyName );
		}

		/// <summary>
		/// Get the casino balance for the user
		/// </summary>
		/// <param name="username">Name of user. Must not be null or empty.</param>
		/// <returns>The balance for the user. Zero if not found</returns>
		private uint GetCurrencyBalance( string username )
        {
            lock ( _lock )
            {
				return _userManager.GetUserCasinoBalance( username );
            }
        }

		/// <summary>
		/// Start the earnings thread
		/// </summary>
        public void Start()
		{
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
            if ( _earningsThread.IsAlive )
			{
				_earningsThread.Abort();
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
