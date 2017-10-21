using System.Text.RegularExpressions;
using BrewBot.Commands.Interfaces;
using BrewBot.Interfaces;
using static BrewBot.CommandParsing.Templates.ACommand;
using System.Collections.Generic;
using BrewBot.Commands.Factory;

namespace BrewBot.Commands
{
	public class CommandFactory : ICommandFactory
    {
		/// <summary>
		/// Create a commandfactory given callbacks to use for command actions.
		/// </summary>
		/// <param name="invalidCommandCB"></param>
		/// <param name="getCommandsCB"></param>
		/// <param name="getBalanceCB"></param>
		/// <param name="gambleCB"></param>
		/// <param name="giveDrinksCB"></param>
		/// <param name="joinGameCB"></param>
		/// <param name="quitGameCB"></param>
		/// <param name="raffleCB"></param>
		/// <param name="splashCB"></param>
		/// <param name="getTicketsCB"></param>
		/// <param name="getTotalDrinksCB"></param>
        public CommandFactory( CommandCallback invalidCommandCB, CommandCallback getCommandsCB, CommandCallback getBalanceCB, CommandCallback gambleCB, CommandCallback giveDrinksCB,
            CommandCallback joinGameCB, CommandCallback quitGameCB, CommandCallback raffleCB, CommandCallback splashCB, CommandCallback getTicketsCB, CommandCallback getTotalDrinksCB )
        {
			// Assign callbacks
			_invalidCommandCB = invalidCommandCB;
			_getCommandsCB = getCommandsCB;
            _getBalanceCB = getBalanceCB;
            _gambleCB = gambleCB;
            _giveDrinksCB = giveDrinksCB;
            _joinGameCB = joinGameCB;
            _quitGameCB = quitGameCB;
            _raffleCB = raffleCB;
            _splashCB = splashCB;
			_getTicketsCB = getTicketsCB;
			_getTotalDrinksCB = getTotalDrinksCB;

			// Initialize structs
			_getCommandsStruct = new CommandStruct( CommandResources.GetCommands_Description, CommandResources.GetCommands_Regex );
			_casinoGetBalanceStruct = new CommandStruct( CommandResources.Casino_GetBalance_Description, CommandResources.Casino_GetBalance_Regex );
			_casinoGambleStruct = new CommandStruct( CommandResources.Casino_Gamble_Description, CommandResources.Casino_Gamble_Regex );
			_casinoSplashStruct = new CommandStruct( CommandResources.Casino_Splash_Description, CommandResources.Casino_Splash_Regex );
			_drinkingGameGiveStruct = new CommandStruct( CommandResources.DrinkingGame_GiveDrink_Description, CommandResources.DrinkingGame_GiveDrink_Regex );
			_drinkingGameJoinStruct = new CommandStruct( CommandResources.DrinkingGame_Join_Description, CommandResources.DrinkingGame_Join_Regex );
			_drinkingGameQuitStruct = new CommandStruct( CommandResources.DrinkingGame_Quit_Description, CommandResources.DrinkingGame_Quit_Regex );
			_raffleStruct = new CommandStruct( CommandResources.Raffle_Description, CommandResources.Raffle_Regex );
			_getDrinkTicketsStruct = new CommandStruct( CommandResources.DrinkingGame_GetTickets_Description, CommandResources.DrinkingGame_GetTickets_Regex );
			_getDrinksStruct = new CommandStruct( CommandResources.DrinkingGame_GetDrinks_Description, CommandResources.DrinkingGame_GetDrinks_Regex );

			// Initiate commands list, order represents how they are displayed in GetCommands
			_commandList = new List<CommandStruct> { _getCommandsStruct, _casinoGetBalanceStruct, _casinoGambleStruct, _casinoSplashStruct, _drinkingGameGiveStruct, _drinkingGameJoinStruct,
				_drinkingGameQuitStruct, _raffleStruct, _getDrinkTicketsStruct, _getDrinksStruct };
		}

        private const RegexOptions REGEX_OPTIONS = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

		private IList<CommandStruct> _commandList;

		private static CommandStruct _getCommandsStruct;
		private static CommandStruct _casinoGetBalanceStruct;
		private static CommandStruct _casinoGambleStruct;
		private static CommandStruct _casinoSplashStruct;
		private static CommandStruct _drinkingGameGiveStruct;
		private static CommandStruct _drinkingGameJoinStruct;
		private static CommandStruct _drinkingGameQuitStruct;
		private static CommandStruct _raffleStruct;
		private static CommandStruct _getDrinkTicketsStruct;
		private static CommandStruct _getDrinksStruct;

		// Invalid command is the only one without an associated struct
		private CommandCallback _invalidCommandCB;

		private CommandCallback _getCommandsCB;
        private CommandCallback _getBalanceCB;
        private CommandCallback _gambleCB;
        private CommandCallback _giveDrinksCB;
        private CommandCallback _joinGameCB;
        private CommandCallback _quitGameCB;
        private CommandCallback _raffleCB;
        private CommandCallback _splashCB;
		private CommandCallback _getTicketsCB;
		private CommandCallback _getTotalDrinksCB;

		/// <summary>
		/// Tries to create a command from a given content and from line. Returns null on failure.
		/// </summary>
		/// <param name="content">The string to create the command from. Should not be null or empty.</param>
		/// <param name="sender">The string denoting the sender of the command. May be null or empty.</param>
		/// <param name="senderIsModerator">A flag denoting whether the user who originated the command is a channel moderator.</param>
		/// <returns>Executable ICommand object or nullon failure</returns>
		public ICommand CreateCommand( string content, string sender, bool senderIsModerator )
        {
            Match match;
            ICommand result = null;

			if ( TryGetMatch( _getCommandsStruct.Regex, content, out match ) )
			{
				result = new GetCommandsCommand( content, sender, null, _getCommandsCB );
			}
			else if ( TryGetMatch( _casinoGetBalanceStruct.Regex, content, out match ) )
            {
                result = new GetBalanceCommand( content, sender, null, _getBalanceCB );
            }
            else if ( TryGetMatch( _casinoGambleStruct.Regex, content, out match ) )
            {
                result = new GambleCommand( content, sender, match.Groups[ 1 ].Value, _gambleCB );
            }
			else if ( TryGetMatch( _drinkingGameGiveStruct.Regex, content, out match ) )
            {
                result = new GiveDrinksCommand( content, sender, match.Groups[ 1 ].Value, _giveDrinksCB );
            }
			else if ( TryGetMatch( _drinkingGameJoinStruct.Regex, content, out match ) )
            {
                result = new JoinDrinkingGameCommand( content, sender, match.Groups[ 1 ].Value, _joinGameCB );
            }
			else if ( TryGetMatch( _drinkingGameQuitStruct.Regex, content, out match ) )
            {
                result = new QuitDrinkingGameCommand( content, sender, null, _quitGameCB );
            }
			else if ( TryGetMatch( _raffleStruct.Regex, content, out match ) )
            {
                result = new RaffleCommand( content, sender, null, _raffleCB );
            }
			else if ( TryGetMatch( _casinoSplashStruct.Regex, content, out match ) )
            {
                result = new SplashCurrencyCommand( content, sender, senderIsModerator, match.Groups[ 1 ].Value, _splashCB );
            }
			else if ( TryGetMatch( _getDrinkTicketsStruct.Regex, content, out match ) )
			{
				result = new GetTicketsCommand( content, sender, null, _getTicketsCB );
			}
			else if ( TryGetMatch( _getDrinksStruct.Regex, content, out match ) )
			{
				result = new GetTotalDrinksTakenCommand( content, sender, null, _getTotalDrinksCB );
			}
			else if ( !string.IsNullOrEmpty( content ) && content[ 0 ] == '!' )
			{
				// If a callback was provided for when a command is invalid, call it
				_invalidCommandCB?.Invoke( sender, null );
			}

			return result;
		}

		/// <summary>
		/// Tries to create a command from a given content and from line. Returns null on failure.
		/// </summary>
		/// <param name="content">The string to create the command from. Should not be null or empty.</param>
		/// <param name="sender">The string denoting the sender of the command. May be null or empty.</param>
		/// <returns></returns>
		public ICommand CreateCommand( string content, string from )
		{
			return CreateCommand( content, from, false );
		}

		// Gives a list of commands
		public List<string> GetCommandDescriptionList()
		{
			List<string> commandDescriptions = new List<string>();
			foreach ( CommandStruct commandStr in _commandList )
			{
				commandDescriptions.Add( commandStr.Description );
			}
			return commandDescriptions;
		}

		private struct CommandStruct
		{
			public CommandStruct( string description, string regex )
			{
				Description = description;
				Regex = new Regex( regex, REGEX_OPTIONS );
			}
			
			public readonly string Description;
			public readonly Regex Regex;
		}

		// Try to parse a line and see if it matches a given regex
        private bool TryGetMatch( Regex pattern, string line, out Match match )
        {
			if ( pattern != null && line != null )
			{
				match = pattern.Match( line );
				return match.Success;
			}
			else
			{
				match = null;
				return false;
			}
        }
    }
}