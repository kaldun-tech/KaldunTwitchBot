using TwitchBot.CommandParsing.Templates;

namespace TwitchBot.Commands
{
    class GambleCommand : ACommand
    {
        public GambleCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
