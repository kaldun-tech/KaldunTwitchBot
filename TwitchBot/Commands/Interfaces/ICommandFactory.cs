using TwitchBot.CommandParsing;

namespace TwitchBot.Commands.Interfaces
{
    interface ICommandFactory
    {
        ACommand CreateCommand( string content, string from );
    }
}
