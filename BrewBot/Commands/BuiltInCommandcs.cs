using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
	class BuiltInCommand : ACommand
	{
		public BuiltInCommand( string content, string sender, string target, CommandCallback cb ) : base( content, sender, target, cb )
		{
		}
	}
}
