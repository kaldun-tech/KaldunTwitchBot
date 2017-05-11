using TwitchBot.Interfaces;

namespace TwitchBot.CommandParsing.Templates
{
    public abstract class ACommand : ICommand
    {
        public delegate void CommandCallback( string from, string target );

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
