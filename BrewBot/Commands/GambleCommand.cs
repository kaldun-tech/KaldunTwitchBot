using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
    class GambleCommand : ACommand
    {
		/// <summary>
		/// Gamble an amount of currency
		/// </summary>
		/// <param name="content"></param>
		/// <param name="sender"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public GambleCommand( string content, string sender, string target, CommandCallback cb ) : base( content, sender, target, cb )
        {
        }
    }
}
