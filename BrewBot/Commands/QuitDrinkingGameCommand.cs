using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
    class QuitDrinkingGameCommand : ACommand
    {
		/// <summary>
		/// The user requests to quit the drinking game
		/// </summary>
		/// <param name="content"></param>
		/// <param name="from"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public QuitDrinkingGameCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
