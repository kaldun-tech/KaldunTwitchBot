using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
    class GambleCommand : ACommand
    {
		/// <summary>
		/// Gamble an amount of currency
		/// </summary>
		/// <param name="content"></param>
		/// <param name="from"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public GambleCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
