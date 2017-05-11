using TwitchBot.CommandParsing.Templates;

namespace TwitchBot.Commands.Templates
{
    abstract class AModeratorOnlyCommand : ACommand
    {
        public AModeratorOnlyCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
