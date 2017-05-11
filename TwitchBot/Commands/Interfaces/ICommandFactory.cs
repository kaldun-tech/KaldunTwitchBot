using TwitchBot.Interfaces;

namespace TwitchBot.Commands.Interfaces
{
    public interface ICommandFactory
    {
        ICommand CreateCommand( string content, string from );
    }
}
