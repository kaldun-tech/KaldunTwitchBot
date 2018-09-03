using Microsoft.VisualStudio.TestTools.UnitTesting;
using BrewBot.Commands;
using BrewBot.Interfaces;
using System.Collections.Generic;
using System;

namespace UnitTests
{
	[TestClass]
	public class CommandFactoryTest
	{
		private const string COMMAND_PREFIX = ":";
		private const string BILLY = "billy";
		private static List<Tuple<string, string, string>> _customCommands = new List<Tuple<string, string, string>>
			{ new Tuple<string, string, string>( BILLY, BILLY, BILLY ) };
		private static CommandFactory _mockFactory = new CommandFactory( null, null, null, null, null, null, null, null, null, null, null, null, COMMAND_PREFIX, _customCommands );

		[TestMethod]
		public void TestCreateCommand_Fail()
		{
			// Test commands that should definitely fail
			ICommand c1 = _mockFactory.CreateCommand( null, null );
			ICommand c2 = _mockFactory.CreateCommand( "", null );
			ICommand c3 = _mockFactory.CreateCommand( null, "" );
			ICommand c4 = _mockFactory.CreateCommand( "", "" );
			ICommand c5 = _mockFactory.CreateCommand( "bullshit", "failfish" );

			Assert.IsNull( c1 );
			Assert.IsNull( c2 );
			Assert.IsNull( c3 );
			Assert.IsNull( c4 );
			Assert.IsNull( c5 );
		}

		[TestMethod]
		public void TestGetCommands()
		{
			Assert.IsNotNull( _mockFactory.GetCommandDescriptionList() );
		}

		[TestMethod]
		public void TestCreateCommand_GetCommands()
		{
			TestSimpleCommand( "commands" );
		}

		[TestMethod]
		public void TestCreateCommand_GetBalance()
		{
			TestSimpleCommand( "balance" );
		}

		[TestMethod]
		public void TestCreateCommand_Gamble()
		{
			TestArgumentCommand( "gamble" );
		}

		[TestMethod]
		public void TestCreateCommand_GiveDrinks()
		{
			TestArgumentCommand( "give" );
		}

		[TestMethod]
		public void TestCreateCommand_JoinDrinkingGame()
		{
			TestArgumentCommand( "join" );
		}

		[TestMethod]
		public void TestCreateCommand_QuitDrinkingGame()
		{
			TestSimpleCommand( "quit" );
		}

		[TestMethod]
		public void TestCreateCommand_RaffleCommand()
		{
			TestSimpleCommand( "raffle" );
		}

		[TestMethod]
		public void TestCreateCommand_SplashCurrency()
		{
			TestArgumentCommand( "splash" );
		}

		[TestMethod]
		public void TestCreateCommand_GetTickets()
		{
			TestSimpleCommand( "tickets" );
		}

		[TestMethod]
		public void TestCreateCommand_GetTotalDrinksTaken()
		{
			TestSimpleCommand( "drinks" );
		}

		[TestMethod]
		public void TestCreateCommand_CustomCommand()
		{
			TestCustomCommand( BILLY );
		}

		private void TestSimpleCommand( string commandBase )
		{
			ICommand c1 = _mockFactory.CreateCommand( COMMAND_PREFIX + commandBase, null );
			ICommand c2 = _mockFactory.CreateCommand( COMMAND_PREFIX + commandBase + " yourmom", null );
			ICommand c3 = _mockFactory.CreateCommand( commandBase, null );

			Assert.IsNotNull( c1 );
			Assert.IsNull( c2 );
			Assert.IsNull( c3 );
		}

		private void TestCustomCommand( string commandBase )
		{
			ICommand c1 = _mockFactory.CreateCommand( COMMAND_PREFIX + commandBase, null );
			ICommand c2 = _mockFactory.CreateCommand( commandBase, null );

			Assert.IsNotNull( c1 );
			Assert.IsNull( c2 );
		}

		private void TestArgumentCommand( string commandBase )
		{
			ICommand c1 = _mockFactory.CreateCommand( COMMAND_PREFIX + commandBase + " 100", null );
			ICommand c2 = _mockFactory.CreateCommand( COMMAND_PREFIX + commandBase + " hhh", null );
			ICommand c3 = _mockFactory.CreateCommand( commandBase, null );
			ICommand c4 = _mockFactory.CreateCommand( COMMAND_PREFIX + commandBase + "fail", null );

			Assert.IsNotNull( c1 );
			Assert.IsNotNull( c2 );
			Assert.IsNull( c3 );
			Assert.IsNull( c4 );
		}
	}
}
