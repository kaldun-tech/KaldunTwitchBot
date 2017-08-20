using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
    class RaffleCommand : ACommand
    {
		/// <summary>
		/// Enter a raffle
		/// </summary>
		/// <param name="content"></param>
		/// <param name="from"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public RaffleCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
