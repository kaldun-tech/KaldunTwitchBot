using TwitchBot.CommandParsing;

namespace TwitchBot.Commands
{
    class SplashCurrencyCommand : ACommand
    {
        public SplashCurrencyCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
