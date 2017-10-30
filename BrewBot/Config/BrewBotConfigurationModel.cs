using System.Collections.Generic;

namespace BrewBot.Config
{
	public class BrewBotConfigurationModel
	{
		/// <summary>
		/// Initialize a model with a configuration file
		/// </summary>
		/// <param name="configFilePath"></param>
		public BrewBotConfigurationModel( string configFilePath )
		{
			_config = new BrewBotConfiguration();
			_reader = new ConfigurationReader();
			_writer = new ConfigurationWriter();
			ConfigFilePath = configFilePath;
			HasUnsavedChanges = false;
		}

		/// <summary>
		/// Initialize a model with no inital configuration
		/// </summary>
		public BrewBotConfigurationModel() : this( null )
		{ }

		private BrewBotConfiguration _config = null;
		private ConfigurationReader _reader = null;
		private ConfigurationWriter _writer = null;
		private string _configFilePath;

		/// <summary>
		/// The configuration XML file path
		/// </summary>
		public string ConfigFilePath
		{
			get { return _configFilePath; }
			set
			{
				if ( value != null && value != ConfigFilePath )
				{
					MakeDirty();
					_configFilePath = value;
				}
			}
		}

		/// <summary>
		/// Whether there are modifications to the model that have not been written to file
		/// </summary>
		public bool HasUnsavedChanges { get; private set; }

		/// <summary>
		/// Sets HasUnsavedChanges to true
		/// </summary>
		private void MakeDirty()
		{
			HasUnsavedChanges = true;
		}

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
				// Check the count as a proxy for list equality
				if ( value != null && value != MessagesToSend && value.Count != MessagesToSend.Count )
				{
					_config.MessagesToSend = value;
					MakeDirty();
				}
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
				if ( value != SecondsBetweenMessageSend )
				{
					_config.SecondsBetweenMessageSend = value;
					MakeDirty();
				}
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
				if ( value != null && value != SubscriberTitle )
				{
					_config.SubscriberTitle = value;
					MakeDirty();
				}
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
				if ( value != null && value != CurrencyName )
				{
					_config.CurrencyName = value;
					MakeDirty();
				}
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
				if ( value != CurrencyEarnedPerMinute )
				{
					_config.CurrencyEarnedPerMinute = value;
					MakeDirty();
				}
			}
		}

		/// <summary>
		/// Whether gambling is enabled
		/// </summary>
		public bool IsGamblingEnabled
		{
			get { return _config.IsGamblingEnabled; }
			set
			{
				if ( value != IsGamblingEnabled )
				{
					_config.IsGamblingEnabled = value;
					MakeDirty();
				}
			}
		}

		/// <summary>
		/// The minimum amount that users can gamble
		/// </summary>
		public uint MinimumGambleAmount
		{
			get { return _config.MinimumGambleAmount; }
			set
			{
				if ( value != MinimumGambleAmount )
				{
					_config.MinimumGambleAmount = value;
					MakeDirty();
				}
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
				if ( value !=  MinimumTimeInSecondsBetweenGambles )
				{
					_config.MinimumTimeInSecondsBetweenGambles = value;
					MakeDirty();
				}
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
				if ( value != GambleChanceToWin )
				{
					_config.GambleChanceToWin = value;
					MakeDirty();
				}
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
				if ( value != TimeoutSeconds )
				{
					_config.TimeoutSeconds = value;
					MakeDirty();
				}
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
				// Check the count as a proxy for list equality
				if ( value != null && value != TimeoutWords && value.Count != TimeoutWords.Count )
				{
					_config.TimeoutWords = value;
					MakeDirty();
				}
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
				// Check the count as a proxy for list equality
				if ( value != null && value != BannedWords && value.Count != BannedWords.Count )
				{
					_config.BannedWords = value;
					MakeDirty();
				}
			}
		}

		/// <summary>
		/// Read the configuration from file to populate the model
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
		/// Write the configuration model to file
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
