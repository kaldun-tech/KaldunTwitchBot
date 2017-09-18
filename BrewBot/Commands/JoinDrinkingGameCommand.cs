using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
    class JoinDrinkingGameCommand : ACommand
    {
		/// <summary>
		/// Join a running drinking game
		/// </summary>
		/// <param name="content"></param>
		/// <param name="sender"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public JoinDrinkingGameCommand( string content, string sender, string target, CommandCallback cb ) : base( content, sender, target, cb )
        {
        }
    }
}
