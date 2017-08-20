using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
	class GetTotalDrinksTakenCommand : ACommand
	{
		public GetTotalDrinksTakenCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
		{
		}
	}
}
