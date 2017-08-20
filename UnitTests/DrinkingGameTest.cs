using Microsoft.VisualStudio.TestTools.UnitTesting;
using BrewBot;

namespace UnitTests
{
	[TestClass]
	public class DrinkingGameTest
	{
		private const string ALICE = "Alice";
		private const string BOB = "Bob";
		private const string CHARLIE = "Charlie";
		private const string PLAYER_ONE = "Alpha";
		private const string PLAYER_TWO = "Beta";
		private const string PLAYER_THREE = "Gamma";
		private const string PLAYER_FOUR = "Omega";
		private const uint ZERO = 0;
		private const uint ONE = 1;
		private const uint TWO = 2;
		private const uint THREE = 3;
		private const uint FOUR = 4;

		private UserManager _mockManager;
		private DrinkingGame _game;

		[TestInitialize]
		public void Initialize()
		{
			// Initialize without a csv file so we don't read or write any data
			_mockManager = new UserManager( null );

			// Log in users
			_mockManager.LoginUser( ALICE );
			_mockManager.LoginUser( BOB );
			_mockManager.LoginUser( CHARLIE );

			_game = new DrinkingGame( _mockManager );
		}

		[TestMethod]
		public void TestConstructor()
		{
			Assert.IsFalse( _game.IsPlaying );

			Assert.IsFalse( _game.IsUserPlaying( ALICE ) );
			Assert.IsFalse( _game.IsUserPlaying( BOB ) );
			Assert.IsFalse( _game.IsUserPlaying( CHARLIE ) );

			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), ZERO );

			Assert.AreEqual( _mockManager.GetDrinkTickets( ALICE ), ZERO );
			Assert.AreEqual( _mockManager.GetDrinkTickets( BOB ), ZERO );
			Assert.AreEqual( _mockManager.GetDrinkTickets( CHARLIE ), ZERO );
		}

		[TestMethod]
		public void TestStartStopPlaying()
		{
			_game.StartPlaying( PLAYER_ONE, PLAYER_TWO, PLAYER_THREE, PLAYER_FOUR );
			Assert.IsTrue( _game.IsPlaying );

			_game.StopPlaying();
			Assert.IsFalse( _game.IsPlaying );
		}

		[TestMethod]
		public void TestSetRemoveParticipants()
		{
			_game.StartPlaying( PLAYER_ONE, PLAYER_TWO, PLAYER_THREE, PLAYER_FOUR );

			Assert.IsFalse( _game.IsUserPlaying( ALICE ) );
			Assert.IsFalse( _game.IsUserPlaying( BOB ) );
			Assert.IsFalse( _game.IsUserPlaying( CHARLIE ) );

			_game.SetParticipant( ALICE, 1 );
			_game.SetParticipant( BOB, 2 );
			_game.SetParticipant( CHARLIE, 3 );

			Assert.IsTrue( _game.IsUserPlaying( ALICE ) );
			Assert.IsTrue( _game.IsUserPlaying( BOB ) );
			Assert.IsTrue( _game.IsUserPlaying( CHARLIE ) );

			_game.SetParticipant( ALICE, 2 );
			_game.SetParticipant( BOB, 3 );
			_game.SetParticipant( CHARLIE, 4 );

			Assert.IsTrue( _game.IsUserPlaying( ALICE ) );
			Assert.IsTrue( _game.IsUserPlaying( BOB ) );
			Assert.IsTrue( _game.IsUserPlaying( CHARLIE ) );

			_game.RemoveParticipant( ALICE );
			_game.RemoveParticipant( BOB );
			_game.RemoveParticipant( CHARLIE );

			Assert.IsFalse( _game.IsUserPlaying( ALICE ) );
			Assert.IsFalse( _game.IsUserPlaying( BOB ) );
			Assert.IsFalse( _game.IsUserPlaying( CHARLIE ) );
		}

		[TestMethod]
		public void TestGiveDrinks()
		{
			_game.StartPlaying( PLAYER_ONE, PLAYER_TWO, PLAYER_THREE, PLAYER_FOUR );
			_game.SetParticipant( ALICE, 1 );
			_game.SetParticipant( BOB, 1 );
			_game.SetParticipant( CHARLIE, 3 );

			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), ZERO );

			_game.GivePlayerDrink( ALICE );
			_game.GivePlayerDrink( ALICE );
			_game.GivePlayerDrink( CHARLIE );

			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), TWO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), ONE );

			_game.GivePlayersDrinks( 1 );

			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), THREE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), ONE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), ONE );

			_game.GivePlayersDrinks( 2 );

			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), THREE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), ONE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), ONE );

			_game.GivePlayersDrinks( 3 );

			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), THREE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), ONE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), TWO );

			_game.PlayerFinishDrink( BOB );

			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), THREE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), TWO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), TWO );

			_game.AllPlayersDrink();

			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), FOUR );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), THREE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), THREE );

			_game.RemoveParticipant( ALICE );
			_game.RemoveParticipant( BOB );
			_game.RemoveParticipant( CHARLIE );
		}

		[TestMethod]
		public void TestGiveTickets()
		{
			_game.StartPlaying( PLAYER_ONE, PLAYER_TWO, PLAYER_THREE, PLAYER_FOUR );
			_game.SetParticipant( ALICE, 1 );
			_game.SetParticipant( BOB, 1 );
			_game.SetParticipant( CHARLIE, 3 );

			Assert.AreEqual( _mockManager.GetDrinkTickets( ALICE ), ZERO );
			Assert.AreEqual( _mockManager.GetDrinkTickets( BOB ), ZERO );
			Assert.AreEqual( _mockManager.GetDrinkTickets( CHARLIE ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), ZERO );

			_game.GivePlayersTicket( 1 );
			_game.GivePlayersTicket( 2 );
			_game.GivePlayersTicket( 3 );

			Assert.AreEqual( _mockManager.GetDrinkTickets( ALICE ), ONE );
			Assert.AreEqual( _mockManager.GetDrinkTickets( BOB ), ONE );
			Assert.AreEqual( _mockManager.GetDrinkTickets( CHARLIE ), ONE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), ZERO );

			_game.GivePlayerTicket( BOB );

			Assert.AreEqual( _mockManager.GetDrinkTickets( ALICE ), ONE );
			Assert.AreEqual( _mockManager.GetDrinkTickets( BOB ), TWO );
			Assert.AreEqual( _mockManager.GetDrinkTickets( CHARLIE ), ONE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), ZERO );

			_game.GivePlayerDrink( BOB, CHARLIE );

			Assert.AreEqual( _mockManager.GetDrinkTickets( ALICE ), ONE );
			Assert.AreEqual( _mockManager.GetDrinkTickets( BOB ), ONE );
			Assert.AreEqual( _mockManager.GetDrinkTickets( CHARLIE ), ONE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), ONE );

			_game.GivePlayerDrink( ALICE, BOB );
			_game.GivePlayerDrink( BOB, CHARLIE );
			_game.GivePlayerDrink( CHARLIE, ALICE );

			Assert.AreEqual( _mockManager.GetDrinkTickets( ALICE ), ZERO );
			Assert.AreEqual( _mockManager.GetDrinkTickets( BOB ), ZERO );
			Assert.AreEqual( _mockManager.GetDrinkTickets( CHARLIE ), ZERO );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( ALICE ), ONE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( BOB ), ONE );
			Assert.AreEqual( _mockManager.GetNumberOfDrinksTaken( CHARLIE ), TWO );
		}

		[TestCleanup]
		public void TearDown()
		{
			_mockManager.LogoutUser( ALICE );
			_mockManager.LogoutUser( BOB );
			_mockManager.LogoutUser( CHARLIE );
			_mockManager.Dispose();

			_game.StopPlaying();
			_game.Disconnect();
		}
	}
}
