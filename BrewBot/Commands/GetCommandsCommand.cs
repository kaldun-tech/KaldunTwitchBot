using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
	class GetCommandsCommand : ACommand
	{
		/// <summary>
		/// Get the list of commands
		/// </summary>
		/// <param name="content"></param>
		/// <param name="sender"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
		public GetCommandsCommand( string content, string sender, string target, CommandCallback cb ) : base( content, sender, target, cb )
		{
		}
	}
}
