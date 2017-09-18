using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
    class RaffleCommand : ACommand
    {
		/// <summary>
		/// Enter a raffle
		/// </summary>
		/// <param name="content"></param>
		/// <param name="sender"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public RaffleCommand( string content, string sender, string target, CommandCallback cb ) : base( content, sender, target, cb )
        {
        }
    }
}
