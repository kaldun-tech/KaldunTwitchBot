using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitchBot;
using System.Collections.Generic;

namespace UnitTests
{
	[TestClass]
	public class UserManagerTest
	{
		public const string ALICE = "Alice";
		private const string BOB = "Bob";
		private const string CHARLIE = "Charlie";
		private const uint ZERO = 0;
		private const uint ONE = 1;
		private const uint TWO = 2;
		private const uint THREE = 3;
		private const uint HUNDRED = 100;
		private const uint TWO_HUNDRED = 200;

		private UserManager _mockUserManager;

		[TestInitialize]
		public void Initialize()
		{
			// Initialize without a csv file so we don't read or write any data
			_mockUserManager = new UserManager( null );
		}
		
		[TestMethod]
		public void TestEmpty()
		{
			Assert.AreEqual( _mockUserManager.ActiveUsers.Count, 0 );
		}

		[TestMethod]
		public void TestLoginLogout()
		{
			_mockUserManager.LoginUser( ALICE );
			_mockUserManager.LoginUser( BOB );
			Assert.AreEqual( _mockUserManager.ActiveUsers.Count, 2 );
			Assert.IsTrue( _mockUserManager.IsUserActive( ALICE ) );
			Assert.IsTrue( _mockUserManager.IsUserActive( BOB ) );
			Assert.IsFalse( _mockUserManager.IsUserActive( CHARLIE ) );

			_mockUserManager.LoginUser( null );
			_mockUserManager.LoginUser( "" );
			Assert.AreEqual( _mockUserManager.ActiveUsers.Count, 2 );
			Assert.IsTrue( _mockUserManager.IsUserActive( ALICE ) );
			Assert.IsTrue( _mockUserManager.IsUserActive( BOB ) );
			Assert.IsFalse( _mockUserManager.IsUserActive( CHARLIE ) );

			_mockUserManager.LogoutUser( CHARLIE );
			_mockUserManager.LogoutUser( BOB );
			_mockUserManager.LogoutUser( ALICE );
			Assert.IsFalse( _mockUserManager.IsUserActive( ALICE ) );
			Assert.IsFalse( _mockUserManager.IsUserActive( BOB ) );
			Assert.IsFalse( _mockUserManager.IsUserActive( CHARLIE ) );
		}

		[TestMethod]
		public void TestBalance()
		{
			_mockUserManager.LoginUser( CHARLIE );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( ALICE ), ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( BOB ), ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), ZERO );

			_mockUserManager.DecreaseUserCurrency( CHARLIE, HUNDRED );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), ZERO );

			_mockUserManager.IncreaseUserCurrency( CHARLIE, ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), ZERO );
			_mockUserManager.IncreaseUserCurrency( CHARLIE, HUNDRED );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), HUNDRED );
			_mockUserManager.IncreaseUserCurrency( CHARLIE, HUNDRED );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), TWO_HUNDRED );

			_mockUserManager.DecreaseUserCurrency( CHARLIE, ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), TWO_HUNDRED );

			_mockUserManager.DecreaseUserCurrency( BOB, TWO_HUNDRED );
			_mockUserManager.DecreaseUserCurrency( ALICE, TWO_HUNDRED );
			_mockUserManager.DecreaseUserCurrency( CHARLIE, TWO_HUNDRED );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( ALICE ), ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( BOB ), ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), ZERO );

			_mockUserManager.LogoutUser( CHARLIE );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( ALICE ), ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( BOB ), ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), ZERO );
		}

		[TestMethod]
		public void TestSplash()
		{
			_mockUserManager.LoginUser( ALICE );
			_mockUserManager.LoginUser( BOB );
			_mockUserManager.LoginUser( CHARLIE );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( ALICE ), ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( BOB ), ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), ZERO );

			_mockUserManager.SplashUserCurrency( HUNDRED );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( ALICE ), HUNDRED );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( BOB ), HUNDRED );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), HUNDRED );

			_mockUserManager.SplashUserCurrency( HUNDRED );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( ALICE ), TWO_HUNDRED );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( BOB ), TWO_HUNDRED );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), TWO_HUNDRED );
			
			_mockUserManager.DecreaseUserCurrency( BOB, TWO_HUNDRED );
			_mockUserManager.DecreaseUserCurrency( ALICE, TWO_HUNDRED );
			_mockUserManager.DecreaseUserCurrency( CHARLIE, TWO_HUNDRED );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( ALICE ), ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( BOB ), ZERO );
			Assert.AreEqual( _mockUserManager.GetUserCasinoBalance( CHARLIE ), ZERO );

			_mockUserManager.LogoutUser( ALICE );
			_mockUserManager.LogoutUser( BOB );
			_mockUserManager.LogoutUser( CHARLIE );
		}

		[TestMethod]
		public void TestDrinking()
		{
			List<string> aliceAndBob = new List<string> { ALICE, BOB };

			_mockUserManager.LoginUser( ALICE );
			_mockUserManager.LoginUser( BOB );
			Assert.AreEqual( _mockUserManager.GetDrinkTickets( ALICE ), ZERO );
			Assert.AreEqual( _mockUserManager.GetDrinkTickets( BOB ), ZERO );
			Assert.AreEqual( _mockUserManager.GetNumberOfDrinksTaken( ALICE ), ZERO );
			Assert.AreEqual( _mockUserManager.GetNumberOfDrinksTaken( BOB ), ZERO );

			_mockUserManager.IncrementDrinkTickets( ALICE );
			_mockUserManager.IncrementDrinkTickets( aliceAndBob );
			Assert.AreEqual( _mockUserManager.GetDrinkTickets( ALICE ), TWO );
			Assert.AreEqual( _mockUserManager.GetDrinkTickets( BOB ), ONE);
			Assert.AreEqual( _mockUserManager.GetNumberOfDrinksTaken( ALICE ), ZERO );
			Assert.AreEqual( _mockUserManager.GetNumberOfDrinksTaken( BOB ), ZERO );

			_mockUserManager.GiveDrink( ALICE, ALICE );
			_mockUserManager.GiveDrink( ALICE, BOB );
			_mockUserManager.GiveDrink( BOB, ALICE );
			Assert.AreEqual( _mockUserManager.GetDrinkTickets( ALICE ), ZERO );
			Assert.AreEqual( _mockUserManager.GetDrinkTickets( BOB ), ZERO );
			Assert.AreEqual( _mockUserManager.GetNumberOfDrinksTaken( ALICE ), TWO );
			Assert.AreEqual( _mockUserManager.GetNumberOfDrinksTaken( BOB ), ONE );

			_mockUserManager.IncrementDrinksTaken( BOB );
			_mockUserManager.IncrementDrinksTaken( aliceAndBob );
			Assert.AreEqual( _mockUserManager.GetDrinkTickets( ALICE ), ZERO );
			Assert.AreEqual( _mockUserManager.GetDrinkTickets( BOB ), ZERO );
			Assert.AreEqual( _mockUserManager.GetNumberOfDrinksTaken( ALICE ), THREE );
			Assert.AreEqual( _mockUserManager.GetNumberOfDrinksTaken( BOB ), THREE );


			_mockUserManager.IncrementDrinkTickets( aliceAndBob );
			_mockUserManager.LogoutUser( ALICE );
			_mockUserManager.LogoutUser( BOB );
			Assert.AreEqual( _mockUserManager.GetDrinkTickets( ALICE ), ONE );
			Assert.AreEqual( _mockUserManager.GetDrinkTickets( BOB ), ONE );
			Assert.AreEqual( _mockUserManager.GetNumberOfDrinksTaken( ALICE ), THREE );
			Assert.AreEqual( _mockUserManager.GetNumberOfDrinksTaken( BOB ), THREE );
		}

		[TestCleanup]
		public void Cleanup()
		{
			_mockUserManager.Dispose();
		}
	}
}
