using TwitchBot.CommandParsing;

namespace TwitchBot.Commands
{
    class RaffleCommand : ACommand
    {
        public RaffleCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
