using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
	class GetTicketsCommand : ACommand
	{
		/// <summary>
		/// Check a user's number of avaiable drink tickets
		/// </summary>
		/// <param name="content"></param>
		/// <param name="sender"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
		public GetTicketsCommand( string content, string sender, string target, CommandCallback cb ) : base( content, sender, target, cb )
		{
		}
	}
}