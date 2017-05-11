using TwitchBot.CommandParsing.Templates;

namespace TwitchBot.Commands
{
    class JoinDrinkingGameCommand : ACommand
    {
        public JoinDrinkingGameCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
