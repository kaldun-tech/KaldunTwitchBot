using System;

namespace TwitchBot
{
	internal class DrunkestViewer
	{
		public DrunkestViewer(int characterNum)
		{
			_characterNum = characterNum;
			_tickets = 0;
		}

		private int _characterNum;
		private int _tickets;

		public int CharacterNum
		{
			get { return _characterNum; }
			set { _characterNum = value; }
		}

		public int Tickets
		{
			get { return _tickets; }
			set { _tickets = value; }
		}
	}
}
