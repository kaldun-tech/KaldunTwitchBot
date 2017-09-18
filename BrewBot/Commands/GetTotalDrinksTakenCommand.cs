using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
	class GetTotalDrinksTakenCommand : ACommand
	{
		public GetTotalDrinksTakenCommand( string content, string sender, string target, CommandCallback cb ) : base( content, sender, target, cb )
		{
		}
	}
}
