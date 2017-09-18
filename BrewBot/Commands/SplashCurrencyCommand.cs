using BrewBot.Commands.Templates;

namespace BrewBot.Commands
{
    class SplashCurrencyCommand : AModeratorOnlyCommand
    {
		/// <summary>
		/// Splash all users with a desired amount of currency
		/// </summary>
		/// <param name="content"></param>
		/// <param name="sender"></param>
		/// <param name="senderIsModerator"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public SplashCurrencyCommand( string content, string sender, bool senderIsModerator, string target, CommandCallback cb ) : base( content, sender, senderIsModerator, target, cb )
        {
        }
    }
}
