using TwitchBot.CommandParsing.Templates;

namespace TwitchBot.Commands
{
	class GetTotalDrinksTakenCommand : ACommand
	{
		public GetTotalDrinksTakenCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
		{
		}
	}
}
