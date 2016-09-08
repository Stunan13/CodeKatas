using KartRacing.Interfaces;
using System.Collections.Generic;
using System.Linq;


namespace KartRacing
{
    public class Leaderboard : ILeaderboard
    {
        public SortedDictionary<int, IPlayerScore> Standings { get; set; }

        public Leaderboard()
        {
            Standings = new SortedDictionary<int, IPlayerScore>();
        }

        public void CalculateStandings(Dictionary<Character, int> scores)
        {
            Standings = new SortedDictionary<int, IPlayerScore>();
            var orderedScores = scores.OrderByDescending(s => s.Value).ToArray();
            
            for (var i = 0; i < orderedScores.Length; i++)
            {
                Standings.Add(i + 1, new PlayerScore(orderedScores[i].Key, orderedScores[i].Value));
            }
        }

        public Dictionary<Character, int> GetPlayerScores(IGame game)
        {
            var scores = new Dictionary<Character, int>();
            foreach (var race in game.Races)
            {
                foreach (var player in race.Players)
                {
                    if (!scores.ContainsKey(player.Key))
                    {
                        scores.Add(player.Key, 0);
                    }

                    scores[player.Key] += CalculatePoints(player.Value.Position);
                }
            }

            return scores;
        }

        public int CalculatePoints(int position)
        {
            var points = 0;
            switch (position)
            {
                case 1:
                    points = 9;
                    break;
                case 2:
                    points = 6;
                    break;
                case 3:
                    points = 3;
                    break;
                case 4:
                    points = 1;
                    break;
            }

            return points;
        }
    }
}
