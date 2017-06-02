using TwitchBot.Interfaces;

namespace TwitchBot.Commands.Interfaces
{
    public interface ICommandFactory
    {
		/// <summary>
		/// Tries to create a command from a given content and from line. Returns null on failure.
		/// </summary>
		/// <param name="content">The string to create the command from. Should not be null or empty.</param>
		/// <param name="from">The string denoting the sender of the command. May be null or empty.</param>
		/// <returns></returns>
        ICommand CreateCommand( string content, string from );
    }
}
