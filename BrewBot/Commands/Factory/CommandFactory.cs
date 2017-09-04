﻿using System.Text.RegularExpressions;
using BrewBot.Commands.Interfaces;
using BrewBot.Interfaces;
using static BrewBot.CommandParsing.Templates.ACommand;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;

namespace BrewBot.Commands
{
    public class CommandFactory : ICommandFactory
    {
		/// <summary>
		/// Create a commandfactory given callbacks to use for command actions.
		/// </summary>
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
        public CommandFactory( CommandCallback getCommandsCB, CommandCallback getBalanceCB, CommandCallback gambleCB, CommandCallback giveDrinksCB,
            CommandCallback joinGameCB, CommandCallback quitGameCB, CommandCallback raffleCB, CommandCallback splashCB, CommandCallback getTicketsCB, CommandCallback getTotalDrinksCB )
        {
            _getBalanceCB = getBalanceCB;
            _gambleCB = gambleCB;
            _giveDrinksCB = giveDrinksCB;
            _joinGameCB = joinGameCB;
            _quitGameCB = quitGameCB;
            _raffleCB = raffleCB;
            _splashCB = splashCB;
			_getTicketsCB = getTicketsCB;
			_getTotalDrinksCB = getTotalDrinksCB;

			ReadJSONCommands();
		}

        private const RegexOptions REGEX_OPTIONS = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
		private readonly string JSON_COMMANDS_FILE_PATH = Path.Combine( Environment.CurrentDirectory, "commands.json" );

		private IList<JSONCommand> _jsonCommands = new List<JSONCommand>();
		private static Regex _commandsEx = new Regex( "^!commands$", REGEX_OPTIONS );
        private static Regex _balanceEx = new Regex( "^!balance$", REGEX_OPTIONS );
        private static Regex _gambleEx = new Regex( "^!gamble (.*)$", REGEX_OPTIONS );
        private static Regex _giveEx = new Regex( "^!give (.*)$", REGEX_OPTIONS );
        private static Regex _joinEx = new Regex( "^!join (.*)$", REGEX_OPTIONS );
        private static Regex _quitEx = new Regex( "^!quit$", REGEX_OPTIONS );
        private static Regex _raffleEx = new Regex( "^!raffle$", REGEX_OPTIONS );
        private static Regex _splashEx = new Regex( "^!splash (.*)$", REGEX_OPTIONS );
		private static Regex _ticketsEx = new Regex( "^!tickets$", REGEX_OPTIONS );
		private static Regex _drinksEx = new Regex( "^!drinks$", REGEX_OPTIONS );

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
		/// <param name="from">The string denoting the sender of the command. May be null or empty.</param>
		/// <returns>Executable ICommand object or nullon failure</returns>
		public ICommand CreateCommand( string content, string from )
        {
            Match match;
            ICommand result = null;

            if ( TryGetMatch( _balanceEx, content, out match ) )
            {
                result = new GetBalanceCommand( content, from, null, _getBalanceCB );
            }
            else if ( TryGetMatch( _gambleEx, content, out match ) )
            {
                result = new GambleCommand( content, from, match.Groups[ 1 ].Value, _gambleCB );
            }
			else if ( TryGetMatch( _giveEx, content, out match ) )
            {
                result = new GiveDrinksCommand( content, from, match.Groups[ 1 ].Value, _giveDrinksCB );
            }
			else if ( TryGetMatch( _joinEx, content, out match ) )
            {
                result = new JoinDrinkingGameCommand( content, from, match.Groups[ 1 ].Value, _joinGameCB );
            }
			else if ( TryGetMatch( _quitEx, content, out match ) )
            {
                result = new QuitDrinkingGameCommand( content, from, null, _quitGameCB );
            }
			else if ( TryGetMatch( _raffleEx, content, out match ) )
            {
                result = new RaffleCommand( content, from, null, _raffleCB );
            }
			else if ( TryGetMatch( _splashEx, content, out match ) )
            {
                result = new SplashCurrencyCommand( content, from, match.Groups[ 1 ].Value, _splashCB );
            }
			else if ( TryGetMatch( _ticketsEx, content, out match ) )
			{
				result = new GetTicketsCommand( content, from, null, _getTicketsCB );
			}
			else if ( TryGetMatch( _drinksEx, content, out match ) )
			{
				result = new GetTotalDrinksTakenCommand( content, from, null, _getTotalDrinksCB );
			}

            return result;
		}

		private struct JSONCommand
		{
			//string commandName;
			//string description;
			//Regex regex;
		}

		/// <summary>
		/// Read the commands in from the JSON file
		/// </summary>
		private void ReadJSONCommands()
		{
			//using ( StreamReader reader = new StreamReader( JSON_COMMANDS_FILE_PATH ) )
			//{
			//	string json = reader.ReadToEnd();
			//	if ( json != null )
			//	{
			//		_jsonCommands = JsonConvert.DeserializeObject<List<JSONCommand>>( json );
			//	}
			//}
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