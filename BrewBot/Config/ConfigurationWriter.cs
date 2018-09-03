using System;
using System.IO;
using System.Xml;

namespace BrewBot.Config
{
	internal class ConfigurationWriter
	{
		/// <summary>
		/// Create a configuration writer
		/// </summary>
		public ConfigurationWriter()
		{ }

		/// <summary>
		/// Write the configuration to file as XML
		/// </summary>
		/// <param name="config"></param>
		/// <param name="configFilePath"></param>
		public void WriteConfig( BrewBotConfiguration config, string configFilePath )
		{
			if ( config != null && !string.IsNullOrEmpty( configFilePath ) )
			{
				XmlDocument document = new XmlDocument();
				
				using ( Stream configStream = new FileStream( configFilePath, FileMode.OpenOrCreate, FileAccess.Write ) )
				{
					using ( XmlWriter writer = XmlWriter.Create( configStream ) )
					{
						StartWriteConfigXML( writer );
						WriteMessageSenderConfigXML( config, writer );
						WriteCustomCommandsConfigXML( config, writer );
						WriteSubscriberConfigXML( config, writer );
						WriteCasinoConfigXML( config, writer );
						WriteModerationConfigXML( config, writer );
						EndWriteConfigXml( writer );
					}
				}
			}
		}

		private void StartWriteConfigXML( XmlWriter writer )
		{
			writer.WriteStartDocument();
			writer.WriteStartElement( ConfigurationResources.ConfigTag );
		}

		private void WriteMessageSenderConfigXML( BrewBotConfiguration config, XmlWriter writer )
		{
			// Interval
			writer.WriteStartElement( ConfigurationResources.MessageIntervalTag );
			writer.WriteAttributeString( ConfigurationResources.MessageInterval_WaitTimeAttribute, config.SecondsBetweenMessageSend.ToString() );
			writer.WriteEndElement();

			// Messages
			if ( config.MessagesToSend.Count > 0 )
			{
				writer.WriteStartElement( ConfigurationResources.MessagesTag );
				foreach ( string message in config.MessagesToSend )
				{
					writer.WriteElementString( ConfigurationResources.MessageTag, message );
				}
				writer.WriteEndElement();
			}
		}

		private void WriteCustomCommandsConfigXML( BrewBotConfiguration config, XmlWriter writer )
		{
			if ( config.AreCommandsCustomized )
			{
				// Custom commands
				writer.WriteStartElement( ConfigurationResources.CustomCommandsTag );
				writer.WriteAttributeString( ConfigurationResources.CustomCommands_DefaultPrefixAttribute, config.CustomCommandPrefix );

				foreach ( Tuple<string, string, string> customCommand in config.CustomCommands )
				{
					writer.WriteStartElement( ConfigurationResources.CustomCommands_CommandTag );
					writer.WriteAttributeString( ConfigurationResources.CustomCommands_CommandNameAttribute, customCommand.Item1 );
					writer.WriteAttributeString( ConfigurationResources.CustomCommands_CommandDescriptionAttribute, customCommand.Item2 );
					writer.WriteValue( customCommand.Item3 );
					writer.WriteEndElement();
				}

				// End custom commands
				writer.WriteEndElement();
			}
		}

		private void WriteSubscriberConfigXML( BrewBotConfiguration config, XmlWriter writer )
		{
			if ( !string.IsNullOrEmpty( config.SubscriberTitle ) )
			{
				writer.WriteStartElement( ConfigurationResources.SubscribersTag );
				writer.WriteAttributeString( ConfigurationResources.Subscriber_TitleAttribute, config.SubscriberTitle );
				writer.WriteEndElement();
			}
		}

		private void WriteCasinoConfigXML( BrewBotConfiguration config, XmlWriter writer )
		{
			writer.WriteStartElement( ConfigurationResources.CurrencyTag );

			if ( !string.IsNullOrEmpty( config.CurrencyName ) )
			{
				writer.WriteAttributeString( ConfigurationResources.Currency_NameAttribute, config.CurrencyName );
			}
			if ( config.CurrencyEarnedPerMinute > 0 )
			{
				writer.WriteAttributeString( ConfigurationResources.Currency_EarnRateAttribute, config.CurrencyEarnedPerMinute.ToString() );
			}

			writer.WriteEndElement();

			if ( config.IsGamblingEnabled )
			{
				writer.WriteStartElement( ConfigurationResources.GamblingTag );

				writer.WriteAttributeString( ConfigurationResources.Gambling_MinimumAmountAttribute, config.MinimumGambleAmount.ToString() );
				writer.WriteAttributeString( ConfigurationResources.Gambling_IntervalAttribute, config.MinimumTimeInSecondsBetweenGambles.ToString() );
				writer.WriteAttributeString( ConfigurationResources.Gambling_OddsAttribute, config.GambleChanceToWin.ToString() );

				writer.WriteEndElement();
			}
		}

		private void WriteModerationConfigXML( BrewBotConfiguration config, XmlWriter writer )
		{
			if ( config.IsModerationEnabled )
			{
				writer.WriteStartElement( ConfigurationResources.ModerationTag );
				writer.WriteAttributeString( ConfigurationResources.Moderation_TimeoutTimeAttribute, config.TimeoutSeconds.ToString() );

				if ( config.TimeoutWords.Count > 0 )
				{
					writer.WriteStartElement( ConfigurationResources.Moderation_TimeoutWordsTag );
					foreach ( string word in config.TimeoutWords )
					{
						writer.WriteElementString( ConfigurationResources.Moderation_WordSubtag, word );
					}
					writer.WriteEndElement();
				}
				if ( config.BannedWords.Count > 0 )
				{
					writer.WriteStartElement( ConfigurationResources.Moderation_BannedWordsTag );
					foreach ( string word in config.BannedWords )
					{
						writer.WriteElementString( ConfigurationResources.Moderation_WordSubtag, word );
					}
					writer.WriteEndElement();
				}

				writer.WriteEndElement();
			}
		}

		private void EndWriteConfigXml( XmlWriter writer )
		{
			writer.WriteEndElement();
			writer.WriteEndDocument();
		}
	}
}
