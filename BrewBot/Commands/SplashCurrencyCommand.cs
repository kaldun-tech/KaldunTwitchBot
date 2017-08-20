using BrewBot.Commands.Templates;

namespace BrewBot.Commands
{
    class SplashCurrencyCommand : AModeratorOnlyCommand
    {
		/// <summary>
		/// Splash all users with a desired amount of currency
		/// </summary>
		/// <param name="content"></param>
		/// <param name="from"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public SplashCurrencyCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
