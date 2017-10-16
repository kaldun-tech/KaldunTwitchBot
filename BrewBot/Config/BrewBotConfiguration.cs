using System.Collections.Generic;

namespace BrewBot.Config
{
	internal class BrewBotConfiguration
	{
		public BrewBotConfiguration( string configFile )
		{
			XMLConfigFile = configFile;
			MessagesToSend = new List<string>();
			SecondsBetweenMessageSend = DEFAULT_MESSAGE_INTERVAL;
			SubscriberTitle = DEFAULT_SUBSCRIBER_TITLE;
			IsCurrencyEnabled = false;
			CurrencyName = DEFAULT_CURRENCY_NAME;
			CurrencyEarnedPerMinute = DEFAULT_CURRENCY_EARN_RATE;
			IsGamblingEnabled = false;
			MinimumGambleAmount = DEFAULT_MINIMUM_GAMBLE_AMOUNT;
			MinimumTimeInSecondsBetweenGambles = DEFAULT_MINIMUM_GAMBLE_INTERVAL;
			GambleChanceToWin = DEFAULT_WIN_CHANCE;
			TimeoutSeconds = DEFAULT_TIMEOUT_SECONDS;
			TimeoutWords = new List<string>();
			BannedWords = new List<string>();
		}

		// Default time between messages
		private const int DEFAULT_MESSAGE_INTERVAL = 120;
		// Minimum time between messages
		private const int MINIMUM_MESSAGE_INTERVAL = 30;
		// Default title to address subscribers with
		private const string DEFAULT_SUBSCRIBER_TITLE = "subscriberino";
		// Default currency name
		private const string DEFAULT_CURRENCY_NAME = "cheddar";
		// Default amount of currency earned per minute
		private const uint DEFAULT_CURRENCY_EARN_RATE = 100;
		// Default minimum amount to gamble
		private const int DEFAULT_MINIMUM_GAMBLE_AMOUNT = 50;
		// Default minimum amount of seconds between allowed gamble attempts
		private const int DEFAULT_MINIMUM_GAMBLE_INTERVAL = 60;
		// Default chance to win. The chance to win must be between zero and one.
		private const double DEFAULT_WIN_CHANCE = 0.5;
		// Default time to time users out if they have said a bad word
		private const int DEFAULT_TIMEOUT_SECONDS = 120;

		private bool _gamblingEnabled;
		private int _messageInterval;
		private List<string> _timeoutWords;
		private List<string> _bannedWords;

		/// <summary>
		/// The configuration file to use
		/// </summary>
		public string XMLConfigFile { get; set; }

		/// <summary>
		/// Get a list of configured messages to send
		/// </summary>
		public List<string> MessagesToSend { get; set; }

		/// <summary>
		/// Get the configured interval in seconds between sending messages
		/// </summary>
		public int SecondsBetweenMessageSend
		{
			get { return _messageInterval; }
			set
			{
				if ( value >= MINIMUM_MESSAGE_INTERVAL )
				{
					_messageInterval = value;
				}
			}
		}

		/// <summary>
		/// The title we will use to address our subscribers
		/// </summary>
		public string SubscriberTitle { get; set; }

		/// <summary>
		/// Whether accrual of currency is enabled. This must be enabled gambling to function.
		/// </summary>
		public bool IsCurrencyEnabled { get; set; }

		/// <summary>
		/// Name of the currency
		/// </summary>
		public string CurrencyName { get; set; }

		/// <summary>
		/// Amount of currency users earn per minute while in the channel with the bot running
		/// </summary>
		public uint CurrencyEarnedPerMinute { get; set; }

		/// <summary>
		/// Whether gambling is enabled
		/// </summary>
		public bool IsGamblingEnabled
		{
			get { return IsCurrencyEnabled && _gamblingEnabled; }
			set { _gamblingEnabled = value; }
		}

		/// <summary>
		/// The minimum amount that users can gamble
		/// </summary>
		public uint MinimumGambleAmount { get; set; }

		/// <summary>
		/// Minimum amount of time between gamble commands in seconds
		/// </summary>
		public int MinimumTimeInSecondsBetweenGambles { get; set; }

		/// <summary>
		/// Chance to win. Must be between zero and one inclusive.
		/// </summary>
		public double GambleChanceToWin { get; set; }

		/// <summary>
		/// Check whether automated chat channel moderation is enabled
		/// </summary>
		public bool IsModerationEnabled
		{
			get
			{
				return TimeoutWords.Count > 0 || BannedWords.Count > 0;
			}
		}

		/// <summary>
		/// The number of seconds to timeout users who use bad words
		/// </summary>
		public int TimeoutSeconds { get; set; }

		/// <summary>
		/// The list of words that will cause a user to be timed out
		/// </summary>
		public List<string> TimeoutWords
		{
			get { return _timeoutWords ?? new List<string>(); }
			set
			{
				if ( value != null )
				{
					_timeoutWords = value;
				}
			}
		}

		/// <summary>
		/// The list of words that will cause a user to be banned from chat
		/// </summary>
		public List<string> BannedWords
		{
			get { return _bannedWords ?? new List<string>(); }
			set
			{
				if ( value != null )
				{
					_bannedWords = value;
				}
			}
		}
	}
}