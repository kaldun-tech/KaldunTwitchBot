namespace TwitchBot
{
    internal class DrinkingGameParticipant
    {
		/// <summary>
		/// Create a participant in the drinking game
		/// </summary>
		/// <param name="characterNum">Which character the participant represents</param>
        public DrinkingGameParticipant( int characterNum )
        {
            _characterNum = characterNum;
            _tickets = 0;
        }

        private int _characterNum;
        private int _tickets;

        /// <summary>
        /// Index of the player
        /// </summary>
		public int PlayerNumber
        {
            get { return _characterNum; }
            set { _characterNum = value; }
        }

        /// <summary>
        /// Number of drink tickets the player has to give out
        /// </summary>
		public int NumberOfDrinkTickets
        {
            get { return _tickets; }
            set { _tickets = value; }
        }
    }
}
