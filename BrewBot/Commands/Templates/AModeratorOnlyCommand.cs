using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands.Templates
{
	abstract class AModeratorOnlyCommand : ACommand
	{
		/// <summary>
		/// A moderator only command
		/// </summary>
		/// <param name="content"></param>
		/// <param name="sender"></param>
		/// <param name="senderIsModerator"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
		public AModeratorOnlyCommand( string content, string sender, bool senderIsModerator, string target, CommandCallback cb ) : base( content, sender, target, cb )
		{
			IsModerator = senderIsModerator;
		}

		public bool IsModerator
		{
			get; set;
		}

		public override void ExecuteCommand()
		{
			if ( IsModerator )
			{
				base.ExecuteCommand();
			}
		}
	}
}
