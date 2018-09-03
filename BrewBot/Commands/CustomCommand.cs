using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
	class CustomCommand : ACommand
	{
		/// <summary>
		/// Custom command
		/// </summary>
		/// <param name="content"></param>
		/// <param name="sender"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
		/// <param name="output"></param>
		public CustomCommand( string content, string sender, string target, CommandCallback cb, string output ) : base( content, sender, target, cb )
		{
			Output = output;
		}
	}
}
