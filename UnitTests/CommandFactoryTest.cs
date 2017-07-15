using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitchBot.Commands;
using TwitchBot.Interfaces;

namespace UnitTests
{
	[TestClass]
	public class CommandFactoryTest
	{
		private readonly CommandFactory _mockFactory = new CommandFactory( null, null, null, null, null, null, null, null, null );

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

		private void TestSimpleCommand( string commandBase )
		{
			ICommand c1 = _mockFactory.CreateCommand( "!" + commandBase, null );
			ICommand c2 = _mockFactory.CreateCommand( "!" + commandBase + " yourmom", null );

			Assert.IsNotNull( c1 );
			Assert.IsNull( c2 );
		}

		private void TestArgumentCommand( string commandBase )
		{
			ICommand c1 = _mockFactory.CreateCommand( "!" + commandBase + " 100", null );
			ICommand c2 = _mockFactory.CreateCommand( "!" + commandBase + " hhh", null );
			ICommand c3 = _mockFactory.CreateCommand( commandBase, null );
			ICommand c4 = _mockFactory.CreateCommand( "!" + commandBase + "fail", null );

			Assert.IsNotNull( c1 );
			Assert.IsNotNull( c2 );
			Assert.IsNull( c3 );
			Assert.IsNull( c4 );
		}
	}
}
