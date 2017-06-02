using TwitchBot.Interfaces;

namespace TwitchBot.CommandParsing.Templates
{
    public abstract class ACommand : ICommand
    {
		/// <summary>
		/// Callback to be executed by the command
		/// </summary>
		/// <param name="from"></param>
		/// <param name="target"></param>
        public delegate void CommandCallback( string from, string target );

		/// <summary>
		/// Create a command
		/// </summary>
		/// <param name="content">Line to create the command with. Must not be null or empty</param>
		/// <param name="from">Who sent the command</param>
		/// <param name="target">What the command is targeting</param>
		/// <param name="cb">Callback to call</param>
        public ACommand( string content, string from, string target, CommandCallback cb )
        {
            Content = content;
            From = from;
            Target = target;
            CB = cb;
        }

        internal string Content
        {
            get; set;
        }

        internal string From
        {
            get; set;
        }

        internal string Target
        {
            get; set;
        }

        internal CommandCallback CB
        {
            get; set;
        }

        public virtual void ExecuteCommand()
        {
            CB( From, Target );
        }
    }
}
