using TwitchBot.CommandParsing.Templates;

namespace TwitchBot.Commands
{
    class CheckBalanceCommand : ACommand
    {
        public CheckBalanceCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
