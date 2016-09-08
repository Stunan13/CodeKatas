
using System;

namespace KartRacing
{
    public static class Constants
    {
        public const int NumLaps = 3;
        public const int TournamentNumRaces = 4;
        public const int SingleRaceNumRaces = 1;
        public const int FirstPlacePoints = 9;
        public const int SecondPlacePoints = 6;
        public const int ThirdPlacePoints = 3;
        public const int FourthPlacePoints = 1;

        public static int NumCharacters
        {
            get
            {
                return Enum.GetNames(typeof(Character)).Length;
            }
        }
    }
}
