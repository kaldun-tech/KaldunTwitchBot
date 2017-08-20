using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
    class GetBalanceCommand : ACommand
    {
		/// <summary>
		/// Check a user's currency balance
		/// </summary>
		/// <param name="content"></param>
		/// <param name="from"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public GetBalanceCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
