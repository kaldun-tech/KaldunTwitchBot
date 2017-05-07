using TwitchBot.CommandParsing;

namespace TwitchBot.Commands
{
    class GiveDrinksCommand : ACommand
    {
        public GiveDrinksCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
