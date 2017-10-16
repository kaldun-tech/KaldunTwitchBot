using Microsoft.VisualStudio.TestTools.UnitTesting;
using BrewBot;

namespace UnitTests
{
	/// <summary>
	/// Summary description for CasinoTest
	/// </summary>
	[TestClass]
	public class CasinoTest
	{
		private const string ALICE = "Alice";
		private const string BOB = "Bob";
		private const string CHARLIE = "Charlie";
		private const uint ZERO = 0;
		private const uint FIFTY = 50;
		private const uint HUNDRED = 100;
		private const uint HUNDRED_FIFTY = 150;

		private UserManager _mockManager;
		private Casino _casino;

		[TestInitialize]
		public void Initialize()
		{
			// Initialize without a csv file so we don't read or write any data
			_mockManager = new UserManager( null );

			// Log in users
			_mockManager.LoginUser( ALICE );
			_mockManager.LoginUser( BOB );
			_mockManager.LoginUser( CHARLIE );

			// Initialize Alice to 50 and Bob to 100. Charlie stays at 0
			_mockManager.IncreaseUserCurrency( ALICE, FIFTY );
			_mockManager.IncreaseUserCurrency( BOB, HUNDRED );

			_casino = new Casino( _mockManager, "dollars", 0, FIFTY, 1 );
		}

		[TestMethod]
		public void TestConstructor()
		{
			// Alice has 50
			Assert.IsTrue( _casino.GetStringBalance( ALICE ).Contains( FIFTY.ToString() ) );
			Assert.AreEqual( _casino.CanUserGamble( ALICE, ZERO ), Casino.CanGambleResult.BELOW_MINIMUM_BET );
			Assert.AreEqual( _casino.CanUserGamble( ALICE, FIFTY ), Casino.CanGambleResult.CAN_GAMBLE );
			Assert.AreEqual( _casino.CanUserGamble( ALICE, HUNDRED ), Casino.CanGambleResult.INSUFFICIENT_FUNDS );

			// Bob has 100
			Assert.IsTrue( _casino.GetStringBalance( BOB ).Contains( HUNDRED.ToString() ) );
			Assert.AreEqual( _casino.CanUserGamble( BOB, FIFTY ), Casino.CanGambleResult.CAN_GAMBLE );
			Assert.AreEqual( _casino.CanUserGamble( BOB, HUNDRED ), Casino.CanGambleResult.CAN_GAMBLE );
			Assert.AreEqual( _casino.CanUserGamble( BOB, HUNDRED_FIFTY ), Casino.CanGambleResult.INSUFFICIENT_FUNDS );

			// Charlie's got nothing
			Assert.IsFalse( _casino.GetStringBalance( CHARLIE ).Contains( FIFTY.ToString() ) );
			Assert.IsFalse( _casino.GetStringBalance( CHARLIE ).Contains( HUNDRED.ToString() ) );
			Assert.IsFalse( _casino.GetStringBalance( CHARLIE ).Contains( HUNDRED_FIFTY.ToString() ) );
			Assert.AreEqual( _casino.CanUserGamble( CHARLIE, FIFTY ), Casino.CanGambleResult.INSUFFICIENT_FUNDS );
		}

		[TestMethod]
		public void TestGamble()
		{
			// Charlie can't gamble
			Assert.IsTrue( _casino.Gamble( CHARLIE, ZERO ).Contains( "at least" ) );
			Assert.IsTrue( _casino.Gamble( CHARLIE, FIFTY ).Contains( "insufficient" ) );

			// Alice can gamble 50
			Assert.IsTrue( _casino.Gamble( ALICE, HUNDRED ).Contains( "insufficient" ) );
			Assert.IsTrue( _casino.Gamble( ALICE, FIFTY ).Contains( "won 50" ) );
			Assert.IsTrue( _casino.GetStringBalance( ALICE ).Contains( HUNDRED.ToString() ) );

			// Bob can gamble 100
			Assert.IsTrue( _casino.Gamble( BOB, HUNDRED_FIFTY ).Contains( "insufficient" ) );
			Assert.IsTrue( _casino.Gamble( BOB, FIFTY ).Contains( "won 50" ) );
			Assert.IsTrue( _casino.GetStringBalance( BOB ).Contains( HUNDRED_FIFTY.ToString() ) );
		}

		[TestMethod]
		public void TestSplash()
		{
			Assert.IsTrue( _casino.SplashUsers( FIFTY ) );
			Assert.IsTrue( _casino.GetStringBalance( ALICE ).Contains( HUNDRED.ToString() ) );
			Assert.IsTrue( _casino.GetStringBalance( BOB ).Contains( HUNDRED_FIFTY.ToString() ) );
			Assert.IsTrue( _casino.GetStringBalance( CHARLIE ).Contains( FIFTY.ToString() ) );

			// Check that zero returns false
			Assert.IsFalse( _casino.SplashUsers( 0 ) );
		}

		[TestCleanup]
		public void TearDown()
		{
			_mockManager.LogoutUser( ALICE );
			_mockManager.LogoutUser( BOB );
			_mockManager.LogoutUser( CHARLIE );
			_mockManager.Dispose();
		}
	}
}
