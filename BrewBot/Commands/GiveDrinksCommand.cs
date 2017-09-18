using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
    class GiveDrinksCommand : ACommand
    {
		/// <summary>
		/// Give drinks to a target user
		/// </summary>
		/// <param name="content"></param>
		/// <param name="sender"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
        public GiveDrinksCommand( string content, string sender, string target, CommandCallback cb ) : base( content, sender, target, cb )
        {
        }
    }
}
