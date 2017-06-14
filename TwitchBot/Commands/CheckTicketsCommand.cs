using TwitchBot.CommandParsing.Templates;

namespace TwitchBot.Commands
{
	class CheckTicketsCommand : ACommand
	{
		/// <summary>
		/// Check a user's number of avaiable drink tickets
		/// </summary>
		/// <param name="content"></param>
		/// <param name="from"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
		public CheckTicketsCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
		{
		}
	}
}