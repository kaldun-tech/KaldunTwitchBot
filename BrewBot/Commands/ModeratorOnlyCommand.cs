using BrewBot.CommandParsing.Templates;

namespace BrewBot.Commands
{
	class ModeratorOnlyCommand : ACommand
	{
		/// <summary>
		/// A moderator only command
		/// </summary>
		/// <param name="content"></param>
		/// <param name="sender"></param>
		/// <param name="target"></param>
		/// <param name="cb"></param>
		/// <param name="senderIsModerator"></param>
		public ModeratorOnlyCommand( string content, string sender, string target, CommandCallback cb, bool senderIsModerator ) : base( content, sender, target, cb )
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
