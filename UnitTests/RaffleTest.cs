using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TwitchBot;

namespace UnitTests
{
	[TestClass]
	public class RaffleTest
	{
		private readonly List<string> _mockUsers = new List<string>{ "Alice", "Bob", "Cassie" };

		[TestMethod]
		public void TestEmptyRaffle()
		{
			Raffle raffle = new Raffle();
			Assert.IsFalse( raffle.IsUserEntered( _mockUsers[ 0 ] ) );
			Assert.IsFalse( raffle.IsUserEntered( _mockUsers[ 1 ] ) );
			Assert.IsFalse( raffle.IsUserEntered( _mockUsers[ 2 ] ) );
			Assert.IsTrue( raffle.DrawUserIndex() < 0 );
		}

		[TestMethod]
		public void TestUserAdd()
		{
			Raffle raffle = new Raffle();
			raffle.AddUser( _mockUsers[ 0 ] );
			raffle.AddUser( _mockUsers[ 1 ] );
			raffle.AddUser( _mockUsers[ 2 ] );

			// Test with three users
			Assert.IsTrue( raffle.IsUserEntered( _mockUsers[ 0 ] ) );
			Assert.IsTrue( raffle.IsUserEntered( _mockUsers[ 1 ] ) );
			Assert.IsTrue( raffle.IsUserEntered( _mockUsers[ 2 ] ) );

			// Try adding the same users in and test again
			raffle.AddUser( _mockUsers[ 0 ] );
			raffle.AddUser( _mockUsers[ 1 ] );
			raffle.AddUser( _mockUsers[ 2 ] );

			Assert.IsTrue( raffle.IsUserEntered( _mockUsers[ 0 ] ) );
			Assert.IsTrue( raffle.IsUserEntered( _mockUsers[ 1 ] ) );
			Assert.IsTrue( raffle.IsUserEntered( _mockUsers[ 2 ] ) );
		}

		[TestMethod]
		public void TestDrawUser()
		{
			Raffle raffle = new Raffle();
			raffle.AddUser( _mockUsers[ 0 ] );
			raffle.AddUser( _mockUsers[ 1 ] );
			raffle.AddUser( _mockUsers[ 2 ] );

			// Check that we always draw a user within bounds
			for ( int i = 0; i < 30; ++i )
			{
				int index = raffle.DrawUserIndex();
				Assert.IsTrue( index >= 0 );
				Assert.IsTrue( index < _mockUsers.Count );
			}
		}

		[TestMethod]
		public void TestClearUsers()
		{
			Raffle raffle = new Raffle();
			raffle.AddUser( _mockUsers[ 0 ] );
			raffle.AddUser( _mockUsers[ 1 ] );
			raffle.AddUser( _mockUsers[ 2 ] );

			raffle.ClearUsers();

			Assert.IsFalse( raffle.IsUserEntered( _mockUsers[ 0 ] ) );
			Assert.IsFalse( raffle.IsUserEntered( _mockUsers[ 1 ] ) );
			Assert.IsFalse( raffle.IsUserEntered( _mockUsers[ 2 ] ) );
		}
	}
}
