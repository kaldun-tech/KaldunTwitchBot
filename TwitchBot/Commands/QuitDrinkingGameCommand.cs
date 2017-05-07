using TwitchBot.CommandParsing;

namespace TwitchBot.Commands
{
    class QuitDrinkingGameCommand : ACommand
    {
        public QuitDrinkingGameCommand( string content, string from, string target, CommandCallback cb ) : base( content, from, target, cb )
        {
        }
    }
}
