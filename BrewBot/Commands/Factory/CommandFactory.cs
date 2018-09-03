using System.Text.RegularExpressions;
using BrewBot.Commands.Interfaces;
using BrewBot.Interfaces;
using static BrewBot.CommandParsing.Templates.ACommand;
using System.Collections.Generic;
using BrewBot.Commands.Factory;
using System;
using BrewBot.CommandParsing.Templates;

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
		/// <param name="customCB"></param>
		public CommandFactory( CommandCallback invalidCommandCB, CommandCallback getCommandsCB, CommandCallback getBalanceCB, CommandCallback gambleCB,
			CommandCallback giveDrinksCB, CommandCallback joinGameCB, CommandCallback quitGameCB, CommandCallback raffleCB, CommandCallback splashCB,
			CommandCallback getTicketsCB, CommandCallback getTotalDrinksCB, CommandCallback customCB ) :
			this( invalidCommandCB, getCommandsCB, getBalanceCB, gambleCB, giveDrinksCB, joinGameCB, quitGameCB, raffleCB, splashCB, getTicketsCB,
				getTotalDrinksCB, customCB, null, null ) {}

		/// <summary>
		/// Create a commandfactory given callbacks to use for command actions. Allows further customization.
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
		/// <param name="customCB"></param>
		/// <param name="commandPrefix"></param>
		/// <param name="customCommands"></param>
		public CommandFactory( CommandCallback invalidCommandCB, CommandCallback getCommandsCB, CommandCallback getBalanceCB, CommandCallback gambleCB,
			CommandCallback giveDrinksCB, CommandCallback joinGameCB, CommandCallback quitGameCB, CommandCallback raffleCB, CommandCallback splashCB, CommandCallback getTicketsCB,
			CommandCallback getTotalDrinksCB, CommandCallback customCB, string commandPrefix, IList<Tuple<string, string, string>> customCommands )
		{
			// On null just use default value
			if ( commandPrefix != null )
			{
				_commandPrefix = commandPrefix;
			}

			// Initialize structs
			CommandStruct getCommandsStruct = new CommandStruct( CommandResources.GetCommands_Regex, CommandResources.GetCommands_Description, getCommandsCB );
			CommandStruct casinoGetBalanceStruct = new CommandStruct( CommandResources.Casino_GetBalance_Regex, CommandResources.Casino_GetBalance_Description, getBalanceCB );
			CommandStruct casinoGambleStruct = new CommandStruct( CommandResources.Casino_Gamble_Regex, CommandResources.Casino_Gamble_Description, gambleCB );
			CommandStruct casinoSplashStruct = new CommandStruct( CommandResources.Casino_Splash_Regex, CommandResources.Casino_Splash_Description, splashCB, null, true );
			CommandStruct drinkingGameGiveStruct = new CommandStruct( CommandResources.DrinkingGame_GiveDrink_Regex, CommandResources.DrinkingGame_GiveDrink_Description, giveDrinksCB );
			CommandStruct drinkingGameJoinStruct = new CommandStruct( CommandResources.DrinkingGame_Join_Regex, CommandResources.DrinkingGame_Join_Description, joinGameCB );
			CommandStruct drinkingGameQuitStruct = new CommandStruct( CommandResources.DrinkingGame_Quit_Regex, CommandResources.DrinkingGame_Quit_Description, quitGameCB );
			CommandStruct raffleStruct = new CommandStruct( CommandResources.Raffle_Regex, CommandResources.Raffle_Description, raffleCB );
			CommandStruct getDrinkTicketsStruct = new CommandStruct( CommandResources.DrinkingGame_GetTickets_Regex, CommandResources.DrinkingGame_GetTickets_Description, getTicketsCB );
			CommandStruct getDrinksStruct = new CommandStruct( CommandResources.DrinkingGame_GetDrinks_Regex, CommandResources.DrinkingGame_GetDrinks_Description, getTotalDrinksCB );

			// Invalid commands don't have an associated struct and the custom is one callback to many structs
			_invalidCommandCB = invalidCommandCB;
			_customCB = customCB;

			// Initiate commands list, order represents how they are displayed in GetCommands
			_commandList = new List<CommandStruct> { getCommandsStruct, casinoGetBalanceStruct, casinoGambleStruct, casinoSplashStruct, drinkingGameGiveStruct, drinkingGameJoinStruct,
				drinkingGameQuitStruct, raffleStruct, getDrinkTicketsStruct, getDrinksStruct };

			// Add any custom commands. The list contains structures with name, description, and output.
			SetCustomCommands( customCommands, true );
		}

		private const RegexOptions REGEX_OPTIONS = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

		private static string _commandPrefix = CommandResources.Default_Prefix;

		private IList<CommandStruct> _commandList;
		private CommandCallback _invalidCommandCB;
		private CommandCallback _customCB;

		/// <summary>
		/// Get the sequence of characters that precedes a command. An input of null resets the default value.
		/// </summary>
		public static string CommandPrefix
		{
			get { return _commandPrefix; }
			set
			{
				if ( value == null )
				{
					_commandPrefix = CommandResources.Default_Prefix;
				}
				else
				{
					_commandPrefix = value;
				}
			}
		}

		/// <summary>
		/// Set custom commands
		/// </summary>
		/// <param name="customCommands"></param>
		public void SetCustomCommands( IList<Tuple<string, string, string>> customCommands, bool initialSetup = false )
		{
			if ( initialSetup )
			{
				// Don't worry about clearing any old custom commands when we first create the factory
				foreach ( CommandStruct commandStruct in _commandList )
				{
					if ( commandStruct.IsCustom )
					{
						_commandList.Remove( commandStruct );
					}
				}
			}
			if ( customCommands != null )
			{
				foreach ( Tuple<string, string, string> customCommand in customCommands )
				{
					// Custom commands are currently never moderator only.
					string name = "^{0}" + customCommand.Item1;
					string description = string.Format( CommandResources.CustomCommand_Description, CommandPrefix, customCommand.Item1, customCommand.Item2 );
					_commandList.Add( new CommandStruct( name, description, _customCB, customCommand.Item3, false ) );
				}
			}
		}

		/// <summary>
		/// Tries to create a command from a given content and from line. Returns null on failure.
		/// </summary>
		/// <param name="content">The string to create the command from. Should not be null or empty.</param>
		/// <param name="sender">The string denoting the sender of the command. May be null or empty.</param>
		/// <param name="senderIsModerator">A flag denoting whether the user who originated the command is a channel moderator.</param>
		/// <returns>Executable ICommand object or nullon failure</returns>
		public ICommand CreateCommand( string content, string sender, bool senderIsModerator )
		{
			foreach ( CommandStruct nextCmd in _commandList )
			{
				if ( TryGetMatch( nextCmd.Regex, content, out Match match ) )
				{
					// Build the command
					return BuildCommand( nextCmd, match, content, sender, senderIsModerator );
				}
			}
			if ( !string.IsNullOrEmpty( content ) && content.StartsWith( CommandPrefix ) )
			{
				// If a callback was provided for when a command is invalid, call it
				_invalidCommandCB?.Invoke( sender, null );
			}

			return null;
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
			/// <summary>
			/// Create a built-in command with the input regex format, description, and callback.
			/// </summary>
			/// <param name="regexFormat"></param>
			/// <param name="description"></param>
			/// <param name="callback"></param>
			public CommandStruct( string regexFormat, string description, CommandCallback callback ) : this( regexFormat, description, callback, null, false )
			{
			}

			/// <summary>
			/// Create a built-in command with the input regex format, description, and custom output. Relies on the CommandPrefix.
			/// </summary>
			/// <param name="regexFormat"></param>
			/// <param name="description"></param>
			/// <param name="callback"></param>
			/// <param name="customOutput"></param>
			public CommandStruct( string regexFormat, string description, CommandCallback callback, string customOutput, bool moderatorOnly )
			{
				DescriptionFormat = description;
				Regex = new Regex( string.Format( regexFormat, CommandPrefix ), REGEX_OPTIONS );
				CB = callback;
				CustomOutput = customOutput;
				IsModeratorOnly = false;
			}

			public readonly Regex Regex;
			public readonly string DescriptionFormat;
			public readonly CommandCallback CB;
			public readonly string CustomOutput;
			public readonly bool IsModeratorOnly;

			public string Description
			{
				get { return string.Format( DescriptionFormat, CommandPrefix ); }
			}

			public bool IsCustom { get { return CustomOutput != null; } }
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

		// Do the actual factory work
		private ACommand BuildCommand( CommandStruct commandStruct, Match match, string content, string sender, bool senderIsModerator )
		{
			string target = match.Groups.Count > 1 ? match.Groups[ 1 ].Value : null;
			if ( commandStruct.IsModeratorOnly )
			{
				return new ModeratorOnlyCommand( content, sender, target, commandStruct.CB, senderIsModerator );
			}
			else if ( commandStruct.CustomOutput != null )
			{
				return new CustomCommand( content, sender, target, commandStruct.CB, commandStruct.CustomOutput );
			}
			else
			{
				return new BuiltInCommand( content, sender, target, commandStruct.CB );
			}
		}
	}
}