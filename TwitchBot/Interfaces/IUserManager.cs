using System.Collections.Generic;

namespace TwitchBot
{
	public interface IUserManager
	{
		IList<string> ActiveUsers { get; }

		void DecreaseUserCurrency( string userName, uint amount );
		void Dispose();
		uint GetDrinkTickets( string userName );
		uint GetNumberOfDrinksTaken( string userName );
		uint GetUserCasinoBalance( string userName );
		void GiveDrink( string sourceUserName, string targetUserName );
		void IncreaseUserCurrency( string userName, uint amount );
		void IncrementDrinksTaken( string userName );
		void IncrementDrinksTaken( ICollection<string> usernames );
		void IncrementDrinkTickets( string userName );
		void IncrementDrinkTickets( ICollection<string> usernames );
		bool IsUserActive( string userName );
		void LoginUser( string userName );
		void LogoutUser( string userName );
		void Reconnect();
		void SplashUserCurrency( uint amount );
	}
}