using System.IO;
using System.Xml;

namespace BrewBot.Config
{
	internal class ConfigurationReader
    {
		/// <summary>
		/// Create a configuration reader
		/// </summary>
        public ConfigurationReader()
        { }

		/// <summary>
		/// Read the config file to parse values and generate a configuration object
		/// </summary>
		/// <param name="configFilePath">Path of the XML configuration file to open</param>
		/// <returns>New configuration read from file</returns>
		public BrewBotConfiguration ReadConfig( string configFilePath)
        {
            if ( !string.IsNullOrEmpty( configFilePath ) )
			{
				XmlDocument document = new XmlDocument();
				using ( Stream configStream = new FileStream( configFilePath, FileMode.Open, FileAccess.Read ) )
				{
					document.Load( configStream );
				}

				BrewBotConfiguration config = new BrewBotConfiguration( configFilePath );
				ReadMessageSenderConfig( config, document );
				ReadSubscriberConfig( config, document );
				ReadCasinoConfig( config, document );
				ReadModerationConfig( config, document );

				return config;
			}

			return null;
        }

		private const string PATH_SEPARATOR = "/";

		// Build all the paths!
		private static string configPath = PATH_SEPARATOR + ConfigurationResources.ConfigTag;
		private static string subscriberPath = configPath + PATH_SEPARATOR + ConfigurationResources.SubscribersTag;
		private static string intervalPath = configPath + PATH_SEPARATOR + ConfigurationResources.MessageIntervalTag;
		private static string messagesPath = configPath + PATH_SEPARATOR + ConfigurationResources.MessagesTag + PATH_SEPARATOR + ConfigurationResources.MessageTag;
		private static string currencyPath = configPath + PATH_SEPARATOR + ConfigurationResources.CurrencyTag;
		private static string gamblingPath = configPath + PATH_SEPARATOR + ConfigurationResources.GamblingTag;
		private static string moderationPath = configPath + PATH_SEPARATOR + ConfigurationResources.ModerationTag;
		private static string moderationWordSubPath = PATH_SEPARATOR + ConfigurationResources.Moderation_WordSubtag;
		private static string timeoutWordsPath = moderationPath + PATH_SEPARATOR + ConfigurationResources.Moderation_TimeoutWordsTag + moderationWordSubPath;
		private static string bannedWordsPath = moderationPath + PATH_SEPARATOR + ConfigurationResources.Moderation_BannedWordsTag + moderationWordSubPath;

		private void ReadMessageSenderConfig( BrewBotConfiguration config, XmlDocument document )
		{
			XmlNode intervalNode = document.DocumentElement.SelectSingleNode( intervalPath );
			XmlAttributeCollection attributes;
			XmlNode waitTimeNode;
			if ( intervalNode != null && ( attributes = intervalNode.Attributes ) != null &&
				( waitTimeNode = attributes.GetNamedItem( ConfigurationResources.MessageInterval_WaitTimeAttribute ) ) != null )
			{
				string value = waitTimeNode.Value;
				config.SecondsBetweenMessageSend = int.Parse( value );
			}

			XmlNodeList messagesList = document.DocumentElement.SelectNodes( messagesPath );
			if ( messagesList != null )
			{
				foreach ( XmlNode messageNode in messagesList )
				{
					if ( messageNode != null && !string.IsNullOrEmpty( messageNode.InnerText ) )
					{
						config.MessagesToSend.Add( messageNode.InnerText );
					}
				}
			}
		}

		private void ReadSubscriberConfig( BrewBotConfiguration config, XmlDocument document )
		{
			XmlNode subscriberNode = document.DocumentElement.SelectSingleNode( subscriberPath );
			if ( subscriberNode != null && subscriberNode.Attributes != null )
			{
				XmlNode titleNode = subscriberNode.Attributes.GetNamedItem( ConfigurationResources.Subscriber_TitleAttribute );
				if ( titleNode != null && !string.IsNullOrEmpty( titleNode.Value ) )
				{
					config.SubscriberTitle = titleNode.Value;
				}
			}
		}

		private void ReadCasinoConfig( BrewBotConfiguration config, XmlDocument document )
		{
			XmlAttributeCollection attributes;
			XmlNode currencyNode = document.DocumentElement.SelectSingleNode( currencyPath );
			if ( currencyNode != null )
			{
				config.IsCurrencyEnabled = true;
				attributes = currencyNode.Attributes;
				if ( attributes != null )
				{
					XmlNode customNameNode = attributes.GetNamedItem( ConfigurationResources.Currency_NameAttribute );
					XmlNode earnRateNode = attributes.GetNamedItem( ConfigurationResources.Currency_EarnRateAttribute );
					string value;

					if ( customNameNode != null && !string.IsNullOrEmpty( value = customNameNode.Value ) )
					{
						config.CurrencyName = value;
					}
					if ( earnRateNode != null && !string.IsNullOrEmpty( value = earnRateNode.Value ) )
					{
						int earnRate;
						bool parsed = int.TryParse( value, out earnRate );
						if ( parsed && earnRate > 0 )
						{
							config.CurrencyEarnedPerMinute = (uint) earnRate;
						}
					}
				}
			}

			XmlNode gamblingNode = document.DocumentElement.SelectSingleNode( gamblingPath );
			if ( gamblingNode != null )
			{
				config.IsGamblingEnabled = true;
				attributes = gamblingNode.Attributes;
				if ( attributes != null )
				{
					XmlNode minimumGambleAmountNode = attributes.GetNamedItem( ConfigurationResources.Gambling_MinimumAmountAttribute );
					XmlNode gamblingIntervalNode = attributes.GetNamedItem( ConfigurationResources.Gambling_IntervalAttribute );
					XmlNode oddsNode = attributes.GetNamedItem( ConfigurationResources.Gambling_OddsAttribute );
					string value;

					if ( minimumGambleAmountNode != null && !string.IsNullOrEmpty( minimumGambleAmountNode.Value ) )
					{
						value = minimumGambleAmountNode.Value;
						uint gambleMinimum;
						bool parsed = uint.TryParse( value, out gambleMinimum );
						if ( parsed && gambleMinimum > 0 )
						{
							config.MinimumGambleAmount = gambleMinimum;
						}
					}
					if ( gamblingIntervalNode != null && !string.IsNullOrEmpty( gamblingIntervalNode.Value ) )
					{
						value = gamblingIntervalNode.Value;
						uint gamblingWaitInterval;
						bool parsed = uint.TryParse( value, out gamblingWaitInterval );
						if ( parsed && gamblingWaitInterval > 0 )
						{
							config.MinimumTimeInSecondsBetweenGambles = gamblingWaitInterval;
						}
					}
					if ( oddsNode != null && !string.IsNullOrEmpty( oddsNode.Value ) )
					{
						value = oddsNode.Value;
						double odds;
						bool parsed = double.TryParse( value, out odds );
						if ( parsed && odds >= 0 && odds <= 1 )
						{
							config.GambleChanceToWin = odds;
						}
					}
				}
			}
		}

		private void ReadModerationConfig( BrewBotConfiguration config, XmlDocument document )
		{
			
			XmlAttributeCollection attributes;
			XmlNode moderationNode = document.DocumentElement.SelectSingleNode( moderationPath );
			if ( moderationNode != null )
			{
				attributes = moderationNode.Attributes;
				if ( attributes != null )
				{
					XmlNode timeoutNode = attributes.GetNamedItem( ConfigurationResources.Moderation_TimeoutTimeAttribute );
					if ( timeoutNode != null )
					{
						int timeout;
						if ( int.TryParse( timeoutNode.Value, out timeout ) )
						{
							config.TimeoutSeconds = timeout;
						}
					}
				}

				XmlNodeList timeoutWordNodes = moderationNode.SelectNodes( timeoutWordsPath );
				foreach ( XmlNode timoutNode in timeoutWordNodes )
				{
					if ( !string.IsNullOrEmpty( timoutNode.InnerText ) )
					{
						config.TimeoutWords.Add( timoutNode.InnerText );
					}
				}

				XmlNodeList bannedWordNodes = moderationNode.SelectNodes( bannedWordsPath );
				foreach ( XmlNode bannedNode in bannedWordNodes )
				{
					if ( !string.IsNullOrEmpty( bannedNode.InnerText ) )
					{
						config.BannedWords.Add( bannedNode.InnerText );
					}
				}
			}
		}
    }
}
