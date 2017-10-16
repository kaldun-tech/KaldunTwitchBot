using System.IO;
using System.Xml;

namespace BrewBot.Config
{
	internal class ConfigurationReader
    {
		/// <summary>
		/// Create a configuration reader for a configuration file path
		/// </summary>
		/// <param name="config"></param>
        public ConfigurationReader( BrewBotConfiguration config )
        {
			_config = config;
        }

		private BrewBotConfiguration _config;

		/// <summary>
		/// Read the config file to parse values
		/// </summary>
        public void ReadConfig()
        {
            if ( string.IsNullOrEmpty( _config.XMLConfigFile ) )
            {
                return;
            }

            using ( Stream configStream = new FileStream( _config.XMLConfigFile, FileMode.Open, FileAccess.Read ) )
			{
				XmlDocument document = new XmlDocument();
				document.Load( configStream );

				ReadMessageSenderConfig( document );
				ReadSubscriberConfig( document );
				ReadCasinoConfig( document );
				ReadModerationConfig( document );
			}
        }

		private void ReadMessageSenderConfig( XmlDocument document )
		{
			XmlNode intervalNode = document.DocumentElement.SelectSingleNode( "/config/interval" );
			XmlAttributeCollection attributes;
			XmlNode waitTimeNode;
			if ( intervalNode != null && ( attributes = intervalNode.Attributes ) != null && ( waitTimeNode = attributes.GetNamedItem( "wait-time" ) ) != null )
			{
				string value = waitTimeNode.Value;
				_config.SecondsBetweenMessageSend = int.Parse( value );
			}

			XmlNodeList messagesList = document.DocumentElement.SelectNodes( "/config/messages/message" );
			if ( messagesList != null )
			{
				foreach ( XmlNode messageNode in messagesList )
				{
					if ( messageNode != null && !string.IsNullOrEmpty( messageNode.InnerText ) )
					{
						_config.MessagesToSend.Add( messageNode.InnerText );
					}
				}
			}
		}

		private void ReadSubscriberConfig( XmlDocument document )
		{
			XmlNode subscriberNode = document.DocumentElement.SelectSingleNode( "/config/subscribers" );
			if ( subscriberNode != null && subscriberNode.Attributes != null )
			{
				XmlNode titleNode = subscriberNode.Attributes.GetNamedItem( "title" );
				if ( titleNode != null && !string.IsNullOrEmpty( titleNode.Value ) )
				{
					_config.SubscriberTitle = titleNode.Value;
				}
			}
		}

		private void ReadCasinoConfig( XmlDocument document )
		{
			XmlAttributeCollection attributes;
			XmlNode currencyNode = document.DocumentElement.SelectSingleNode( "/config/currency" );
			if ( currencyNode != null )
			{
				_config.IsCurrencyEnabled = true;
				attributes = currencyNode.Attributes;
				if ( attributes != null )
				{
					XmlNode customNameNode = attributes.GetNamedItem( "custom-name" );
					XmlNode earnRateNode = attributes.GetNamedItem( "earn-rate" );
					string value;

					if ( customNameNode != null && !string.IsNullOrEmpty( value = customNameNode.Value ) )
					{
						_config.CurrencyName = value;
					}
					if ( earnRateNode != null && !string.IsNullOrEmpty( value = earnRateNode.Value ) )
					{
						int earnRate;
						bool parsed = int.TryParse( value, out earnRate );
						if ( parsed && earnRate > 0 )
						{
							_config.CurrencyEarnedPerMinute = (uint) earnRate;
						}
					}
				}
			}

			XmlNode gamblingNode = document.DocumentElement.SelectSingleNode( "/config/gambling" );
			if ( gamblingNode != null )
			{
				_config.IsGamblingEnabled = true;
				attributes = gamblingNode.Attributes;
				if ( attributes != null )
				{
					XmlNode minimumGambleAmountNode = attributes.GetNamedItem( "minimum" );
					XmlNode gamblingFrequencyNode = attributes.GetNamedItem( "frequency" );
					XmlNode oddsNode = attributes.GetNamedItem( "odds" );
					string value;

					if ( minimumGambleAmountNode != null && !string.IsNullOrEmpty( minimumGambleAmountNode.Value ) )
					{
						value = minimumGambleAmountNode.Value;
						uint gambleMinimum;
						bool parsed = uint.TryParse( value, out gambleMinimum );
						if ( parsed && gambleMinimum > 0 )
						{
							_config.MinimumGambleAmount = gambleMinimum;
						}
					}
					if ( gamblingFrequencyNode != null && !string.IsNullOrEmpty( gamblingFrequencyNode.Value ) )
					{
						value = gamblingFrequencyNode.Value;
						int gamblingFrequency;
						bool parsed = int.TryParse( value, out gamblingFrequency );
						if ( parsed && gamblingFrequency > 0 )
						{
							_config.MinimumTimeInSecondsBetweenGambles = gamblingFrequency;
						}
					}
					if ( oddsNode != null && !string.IsNullOrEmpty( oddsNode.Value ) )
					{
						value = oddsNode.Value;
						double odds;
						bool parsed = double.TryParse( value, out odds );
						if ( parsed && odds >= 0 && odds <= 1 )
						{
							_config.GambleChanceToWin = odds;
						}
					}
				}
			}
		}

		private void ReadModerationConfig( XmlDocument document )
		{
			XmlAttributeCollection attributes;
			XmlNode moderationNode = document.DocumentElement.SelectSingleNode( "/config/moderation" );
			if ( moderationNode != null )
			{
				attributes = moderationNode.Attributes;
				if ( attributes != null )
				{
					XmlNode timeoutNode = attributes.GetNamedItem( "timeout-time" );
					if ( timeoutNode != null )
					{
						int timeout;
						if ( int.TryParse( timeoutNode.Value, out timeout ) )
						{
							_config.TimeoutSeconds = timeout;
						}
					}
				}

				XmlNodeList timeoutWordNodes = moderationNode.SelectNodes( "/config/moderation/timeout-words/word" );
				foreach ( XmlNode timoutNode in timeoutWordNodes )
				{
					if ( !string.IsNullOrEmpty( timoutNode.InnerText ) )
					{
						_config.TimeoutWords.Add( timoutNode.InnerText );
					}
				}

				XmlNodeList bannedWordNodes = moderationNode.SelectNodes( "/config/moderation/banned-words/word" );
				foreach ( XmlNode bannedNode in bannedWordNodes )
				{
					if ( !string.IsNullOrEmpty( bannedNode.InnerText ) )
					{
						_config.BannedWords.Add( bannedNode.InnerText );
					}
				}
			}
		}
    }
}
