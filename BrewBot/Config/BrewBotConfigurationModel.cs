using System.Collections.Generic;

namespace BrewBot.Config
{
	public class BrewBotConfigurationModel
	{
		/// <summary>
		/// Initialize a model with no inital configuration
		/// </summary>
		public BrewBotConfigurationModel()
		{
			_config = new BrewBotConfiguration();
			_reader = new ConfigurationReader();
			_writer = new ConfigurationWriter();
			HasUnsavedChanges = false;
		}

		/// <summary>
		/// Initialize a model with a configuration file
		/// </summary>
		/// <param name="configFilePath"></param>
		public BrewBotConfigurationModel( string configFilePath ) : this()
		{
			ConfigFilePath = configFilePath;
		}

		private BrewBotConfiguration _config = null;
		private ConfigurationReader _reader = null;
		private ConfigurationWriter _writer = null;
		private string _configFilePath = null;

		/// <summary>
		/// The configuration XML file path
		/// </summary>
		public string ConfigFilePath
		{
			get { return _configFilePath; }
			set
			{
				HasUnsavedChanges = true;
				_configFilePath = value;
			}
		}

		/// <summary>
		/// Whether there are modifications to the model that have not been written to file
		/// </summary>
		public bool HasUnsavedChanges { get; private set; }

		/// <summary>
		/// Whether sending messages to chat is enabled
		/// </summary>
		public bool IsMessageSendingEnabled
		{
			get { return _config.IsMessageSendingEnabled; }
		}

		/// <summary>
		/// The list of configured messages to send
		/// </summary>
		public List<string> MessagesToSend
		{
			get { return _config.MessagesToSend; }
			set
			{
				_config.MessagesToSend = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// The configured interval in seconds between sending messages
		/// </summary>
		public int SecondsBetweenMessageSend
		{
			get { return _config.SecondsBetweenMessageSend; }
			set
			{
				_config.SecondsBetweenMessageSend = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// The title we will use to address our subscribers
		/// </summary>
		public string SubscriberTitle
		{
			get { return _config.SubscriberTitle; }
			set
			{
				_config.SubscriberTitle = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// Whether accrual of currency is enabled. This must be enabled gambling to function.
		/// </summary>
		public bool IsCurrencyEnabled
		{
			get { return _config.IsCurrencyEnabled; }
			set
			{
				_config.IsCurrencyEnabled = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// Whether gambling is enabled
		/// </summary>
		public string CurrencyName
		{
			get { return _config.CurrencyName; }
			set
			{
				_config.CurrencyName = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// Amount of currency users earn per minute while in the channel with the bot running
		/// </summary>
		public uint CurrencyEarnedPerMinute
		{
			get { return _config.CurrencyEarnedPerMinute; }
			set
			{
				_config.CurrencyEarnedPerMinute = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// Whether gambling is enabled
		/// </summary>
		public bool IsGamblingEnabled
		{
			get { return _config.IsGamblingEnabled; }
			set { _config.IsGamblingEnabled = value; }
		}

		/// <summary>
		/// The minimum amount that users can gamble
		/// </summary>
		public uint MinimumGambleAmount
		{
			get { return _config.MinimumGambleAmount; }
			set
			{
				_config.MinimumGambleAmount = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// Minimum amount of time between gamble commands in seconds
		/// </summary>
		public uint MinimumTimeInSecondsBetweenGambles
		{
			get { return _config.MinimumTimeInSecondsBetweenGambles; }
			set
			{
				_config.MinimumTimeInSecondsBetweenGambles = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// Chance to win. Must be between zero and one inclusive.
		/// </summary>
		public double GambleChanceToWin
		{
			get { return _config.GambleChanceToWin; }
			set
			{
				_config.GambleChanceToWin = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// Check whether automated chat channel moderation is enabled
		/// </summary>
		public bool IsModerationEnabled
		{
			get { return _config.IsModerationEnabled; }
		}

		/// <summary>
		/// The number of seconds to timeout users who use bad words
		/// </summary>
		public int TimeoutSeconds
		{
			get { return _config.TimeoutSeconds; }
			set
			{
				_config.TimeoutSeconds = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// The list of words that will cause a user to be timed out
		/// </summary>
		public List<string> TimeoutWords
		{
			get { return _config.TimeoutWords; }
			set
			{
				_config.TimeoutWords = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// The list of words that will cause a user to be banned from chat
		/// </summary>
		public List<string> BannedWords
		{
			get { return _config.BannedWords; }
			set
			{
				_config.BannedWords = value;
				HasUnsavedChanges = true;
			}
		}

		/// <summary>
		/// Read the configuration from file
		/// </summary>
		public void ReadConfig()
		{
			if ( !string.IsNullOrEmpty( ConfigFilePath ) )
			{
				_config = _reader.ReadConfig( ConfigFilePath );
				HasUnsavedChanges = false;
			}
		}

		/// <summary>
		/// Write the configuration to file
		/// </summary>
		public void WriteConfig()
		{
			if ( !string.IsNullOrEmpty( ConfigFilePath ) )
			{
				_writer.WriteConfig( _config, ConfigFilePath );
				HasUnsavedChanges = false;
			}
		}
	}
}
