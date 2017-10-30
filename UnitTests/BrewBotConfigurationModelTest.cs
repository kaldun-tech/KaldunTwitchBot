using Microsoft.VisualStudio.TestTools.UnitTesting;
using BrewBot.Config;
using System.Collections.Generic;

namespace UnitTests
{
	/// <summary>
	/// Test the BrewBotConfigurationModel class
	/// </summary>
	[TestClass]
	public class BrewBotConfigurationModelTest
	{
		private const string TEST_CONFIG_FILE_NAME = "config_test.xml";
		private const string ALT_CONFIG_FILE_NAME = "other_config.xml";
		private const string SUB_TITLE = "doggos";
		private const string CURRENCY_NAME = "biscuits";
		private readonly List<string> EMPTY_LIST = new List<string>();
		private readonly List<string> MESSAGES = new List<string> { "plz gieb cheezburgr" };
		private readonly List<string> TIMEOUT_WORDS = new List<string> { "noobs" };
		private readonly List<string> BANNED_WORDS = new List<string> { "racial slur" };

		private BrewBotConfigurationModel _model;

		[TestInitialize]
		public void Initialize()
		{
			_model = new BrewBotConfigurationModel( TEST_CONFIG_FILE_NAME );
		}

		/// <summary>
		/// Test that the model was constructed correctly
		/// </summary>
		[TestMethod]
		public void TestConstructor()
		{
			Assert.AreEqual( TEST_CONFIG_FILE_NAME, _model.ConfigFilePath );
			Assert.IsFalse( _model.HasUnsavedChanges );
			Assert.IsFalse( _model.IsMessageSendingEnabled );
			Assert.IsNotNull( _model.MessagesToSend );
			Assert.AreEqual( 0, _model.MessagesToSend.Count );
			Assert.IsNotNull( _model.SubscriberTitle );
			Assert.IsNotNull( _model.CurrencyName );
			Assert.IsFalse( _model.IsGamblingEnabled );
			Assert.IsFalse( _model.IsModerationEnabled );
			Assert.IsNotNull( _model.TimeoutWords );
			Assert.AreEqual( 0, _model.TimeoutWords.Count );
			Assert.IsNotNull( _model.BannedWords );
			Assert.AreEqual( 0, _model.BannedWords.Count );
		}

		/// <summary>
		/// Test that we can make changes to the model and retrieve the correct data
		/// </summary>
		[TestMethod]
		public void TestSetters()
		{
			Assert.AreEqual( false, _model.HasUnsavedChanges );

			// Check that setting to null doesn't work
			_model.ConfigFilePath = null;
			_model.MessagesToSend = null;
			_model.SubscriberTitle = null;
			_model.CurrencyName = null;
			_model.TimeoutWords = null;
			_model.BannedWords = null;

			Assert.IsFalse( _model.HasUnsavedChanges );
			Assert.IsNotNull( _model.ConfigFilePath );
			Assert.IsNotNull( _model.MessagesToSend );
			Assert.IsNotNull( _model.SubscriberTitle );
			Assert.IsNotNull( _model.CurrencyName );
			Assert.IsNotNull( _model.TimeoutWords );
			Assert.IsNotNull( _model.BannedWords );

			// Test that changing properties to the current value does not trigger a change
			_model.ConfigFilePath = TEST_CONFIG_FILE_NAME;
			_model.MessagesToSend = EMPTY_LIST;
			_model.SubscriberTitle = "subscriberino";
			_model.CurrencyName = "cheddar";
			_model.TimeoutWords = EMPTY_LIST;
			_model.BannedWords = EMPTY_LIST;
			Assert.IsFalse( _model.HasUnsavedChanges );

			// Now test actual values

			_model.ConfigFilePath = ALT_CONFIG_FILE_NAME;
			Assert.IsTrue( _model.HasUnsavedChanges );
			Assert.AreEqual( _model.ConfigFilePath, ALT_CONFIG_FILE_NAME );

			_model.MessagesToSend = MESSAGES;
			Assert.AreEqual( _model.MessagesToSend, MESSAGES );

			_model.SubscriberTitle = SUB_TITLE;
			Assert.AreEqual( _model.SubscriberTitle, SUB_TITLE );

			_model.CurrencyName = CURRENCY_NAME;
			Assert.AreEqual( _model.CurrencyName, CURRENCY_NAME );

			_model.TimeoutWords = TIMEOUT_WORDS;
			Assert.AreEqual( _model.TimeoutWords, TIMEOUT_WORDS );

			_model.BannedWords = BANNED_WORDS;
			Assert.AreEqual( _model.BannedWords, BANNED_WORDS );
			Assert.IsTrue( _model.HasUnsavedChanges );
		}

		/// <summary>
		/// Test that we can write the model, read a model, and get the same results back.
		/// </summary>
		[TestMethod]
		public void TestWriteRead()
		{
			_model.ConfigFilePath = TEST_CONFIG_FILE_NAME;
			_model.MessagesToSend = MESSAGES;
			_model.SubscriberTitle = SUB_TITLE;
			_model.CurrencyName = CURRENCY_NAME;
			_model.TimeoutWords = TIMEOUT_WORDS;
			_model.BannedWords = BANNED_WORDS;
			Assert.IsTrue( _model.HasUnsavedChanges );

			_model.WriteConfig();
			Assert.IsFalse( _model.HasUnsavedChanges );

			BrewBotConfigurationModel readModel = new BrewBotConfigurationModel( TEST_CONFIG_FILE_NAME );
			readModel.ReadConfig();
			Assert.IsFalse( _model.HasUnsavedChanges );

			Assert.AreEqual( _model.ConfigFilePath, readModel.ConfigFilePath );
			Assert.AreEqual( _model.MessagesToSend[ 0 ], readModel.MessagesToSend[ 0 ] );
			Assert.AreEqual( _model.SubscriberTitle, readModel.SubscriberTitle );
			Assert.AreEqual( _model.CurrencyName, readModel.CurrencyName );
			Assert.AreEqual( _model.TimeoutWords[ 0 ], readModel.TimeoutWords[ 0 ] );
			Assert.AreEqual( _model.BannedWords[ 0 ], readModel.BannedWords[ 0 ] );
		}
	}
}
