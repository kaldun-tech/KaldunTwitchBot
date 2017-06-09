using TwitchBot.CommandParsing.Templates;

namespace TwitchBot.Commands
{
    class GiveDrinksCommand : ACommand
    {
		/// <summary>
		/// Give drinks to a target user
		/// </summary>
		/// <param name="content"></param>
		/// <param name="from"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public GiveDrinksCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
