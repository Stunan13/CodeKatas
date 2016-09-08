
using System;

namespace KartRacing
{
    public static class Constants
    {
        public const int NumLaps = 3;
        public const int TournamentNumRaces = 4;
        public const int SingleRaceNumRaces = 1;

        public static int NumCharacters
        {
            get
            {
                return Enum.GetNames(typeof(Character)).Length;
            }
        }
    }
}
