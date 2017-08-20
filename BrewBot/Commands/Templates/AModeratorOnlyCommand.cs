using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands.Templates
{
    abstract class AModeratorOnlyCommand : ACommand
    {
		/// <summary>
		/// A moderator only command
		/// </summary>
		/// <param name="content"></param>
		/// <param name="from"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public AModeratorOnlyCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
