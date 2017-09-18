using BrewBot.Interfaces;

namespace BrewBot.Commands.Interfaces
{
    public interface ICommandFactory
    {
		ICommand CreateCommand( string content, string sender );
    }
}
