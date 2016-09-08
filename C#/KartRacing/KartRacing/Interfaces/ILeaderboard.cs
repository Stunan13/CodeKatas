using System.Collections.Generic;

namespace KartRacing.Interfaces
{
    public interface ILeaderboard
    {
        SortedDictionary<int, IPlayerScore> Standings { get; set; }
        void CalculateStandings(Dictionary<Character, int> scores);
        Dictionary<Character, int> GetPlayerScores(IGame game);
        int CalculatePoints(int position);
    }
}
