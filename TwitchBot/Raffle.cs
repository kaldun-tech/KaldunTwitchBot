using System;
using System.Collections.Generic;

namespace TwitchBot
{
	class Raffle
	{
		/// <summary>
		/// Start a new raffle
		/// </summary>
		public Raffle()
		{
			_generator = new Random();
			_participants = new HashSet<string>();
		}

		private Random _generator;
		private HashSet<string> _participants;

		/// <summary>
		/// Check whether a user is entered into the raffle already
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public bool IsUserEntered( string username )
		{
			return _participants.Contains( username );
		}

		/// <summary>
		/// Draw a winner for the raffle, expressed as a position in an collection.
		/// </summary>
		/// <returns>Returns -1 if no users are present, else a user index to draw</returns>
		public int DrawUserIndex()
		{
			return ( _participants.Count > 0 ) ? _generator.Next( _participants.Count ) : -1;
		}

		/// <summary>
		/// Add a user to the raffle
		/// </summary>
		/// <param name="username"></param>
		public void AddUser( string username )
		{
			_participants.Add( username );
		}

		/// <summary>
		/// Clear all entered users from the raffle
		/// </summary>
		public void ClearUsers()
		{
			_participants.Clear();
		}

		/// <summary>
		/// Remove a user from the raffle
		/// </summary>
		/// <param name="username"></param>
		private void RemoveUser( string username )
		{
			if ( !_participants.Contains( username ) )
			{
				_participants.Remove( username );
			}
		}
	}
}
