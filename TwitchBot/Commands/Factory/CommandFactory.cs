using System.Text.RegularExpressions;
using TwitchBot.Commands.Interfaces;
using TwitchBot.Interfaces;
using static TwitchBot.CommandParsing.Templates.ACommand;

namespace TwitchBot.Commands
{
    public class CommandFactory : ICommandFactory
    {
        public CommandFactory( CommandCallback checkBalanceCB, CommandCallback gambleCB, CommandCallback giveDrinksCB,
            CommandCallback joinGameCB, CommandCallback quitGameCB, CommandCallback raffleCB, CommandCallback splashCB )
        {
            _checkBalanceCB = checkBalanceCB;
            _gambleCB = gambleCB;
            _giveDrinksCB = giveDrinksCB;
            _joinGameCB = joinGameCB;
            _quitGameCB = quitGameCB;
            _raffleCB = raffleCB;
            _splashCB = splashCB;
        }

        private const RegexOptions REGEX_OPTIONS = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

        private CommandCallback _checkBalanceCB;
        private CommandCallback _gambleCB;
        private CommandCallback _giveDrinksCB;
        private CommandCallback _joinGameCB;
        private CommandCallback _quitGameCB;
        private CommandCallback _raffleCB;
        private CommandCallback _splashCB;

        private Regex _balanceEx = new Regex( "^!balance$", REGEX_OPTIONS );
        private Regex _gambleEx = new Regex( "^!gamble (.*)$", REGEX_OPTIONS );
        private Regex _giveEx = new Regex( "^!give (.*)$", REGEX_OPTIONS );
        private Regex _joinEx = new Regex( "^!join (.*)$", REGEX_OPTIONS );
        private Regex _quitEx = new Regex( "^!quit$", REGEX_OPTIONS );
        private Regex _raffleEx = new Regex( "^!raffle$", REGEX_OPTIONS );
        private Regex _splashEx = new Regex( "^!splash (.*)$", REGEX_OPTIONS );

        public ICommand CreateCommand( string content, string from )
        {
            Match match;
            ICommand result = null;

            if ( TryGetMatch( _balanceEx, content, out match ) )
            {
                result = new CheckBalanceCommand( content, from, null, _checkBalanceCB );
            }
            if ( TryGetMatch( _gambleEx, content, out match ) )
            {
                result = new GambleCommand( content, from, match.Groups[ 1 ].Value, _gambleCB );
            }
            if ( TryGetMatch( _giveEx, content, out match ) )
            {
                result = new GiveDrinksCommand( content, from, match.Groups[ 1 ].Value, _giveDrinksCB );
            }
            if ( TryGetMatch( _joinEx, content, out match ) )
            {
                result = new JoinDrinkingGameCommand( content, from, match.Groups[ 1 ].Value, _joinGameCB );
            }
            if ( TryGetMatch( _quitEx, content, out match ) )
            {
                result = new QuitDrinkingGameCommand( content, from, null, _quitGameCB );
            }
            if ( TryGetMatch( _raffleEx, content, out match ) )
            {
                result = new RaffleCommand( content, from, null, _raffleCB );
            }
            if ( TryGetMatch( _splashEx, content, out match ) )
            {
                result = new SplashCurrencyCommand( content, from, match.Groups[ 1 ].Value, _splashCB );
            }

            return result;
        }

        private bool TryGetMatch( Regex pattern, string line, out Match match )
        {
            match = pattern.Match( line );
            return match.Success;
        }
    }
}