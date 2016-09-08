using KartRacing.Interfaces;
using System;

namespace KartRacing
{
    public class GameManager
    {
        public IGame Game { get; set; }
        public GameType Type { get; set; }
        public ILeaderboard Leaderboard { get; set; }

        public GameManager()
        {
            
        }

        public GameManager(GameType type, RaceClass raceClass)
        {
            Game = new Game(raceClass);
            Type = type;
            Leaderboard = new Leaderboard();
        }        

        public void AddPlayer(Character character, string name)
        {           
            Game.AddPlayer(character, name);
        }

        public void SelectCourse(Course course)
        {
            var maxRaces = Type == GameType.Tournament ? Constants.TournamentNumRaces : Constants.SingleRaceNumRaces;
            if (Game.Races.Count >= maxRaces)
            {
                throw new ArgumentOutOfRangeException(string.Format("The maximum of: {0} course(s) have already been selected", maxRaces));
            }

            Game.SelectCourse(course);
        }

        public void CreateCpuPlayers()
        {
            var unusedCharacters = Game.GetUnusedCharacters();
            for (int i = 0; i < unusedCharacters.Length; i++)
            {
                string playerName = string.Format("CPU {0}", i + 1);
                AddPlayer(unusedCharacters[i], playerName);
            }
        }

        public void UpdateLeaderboard()
        {
            var scores = Leaderboard.GetPlayerScores(Game);
            Leaderboard.CalculateStandings(scores);
        }
    }
}
