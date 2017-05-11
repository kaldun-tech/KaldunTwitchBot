using TwitchBot.Commands.Templates;

namespace TwitchBot.Commands
{
    class SplashCurrencyCommand : AModeratorOnlyCommand
    {
        public SplashCurrencyCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
