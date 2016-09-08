using KartRacing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KartRacing
{
    public class Game : IGame
    {        
        public List<IRace> Races { get; set; }
        public Dictionary<Character, string> Players { get; set; }
        public RaceClass Class { get; set; }

        public Game(RaceClass raceClass)
        {
            Class = raceClass;
            Races = new List<IRace>();
            Players = new Dictionary<Character, string>();
        }

        public void AddPlayer(Character character, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Player Name cannot be empty");

            if (Players.ContainsKey(character))
                throw new ArgumentException("Character already selected.");

            if (Players.ToList().Any(p => p.Value == name))
                throw new ArgumentException(string.Format("Player: {0} already entered.", name));

            Players.Add(character, name);
        }

        public Character[] GetUnusedCharacters()
        {
            var characters = Enum.GetValues(typeof(Character))
                                       .Cast<Character>()
                                       .ToArray();

            return characters.Where(c => !Players.Keys.Contains(c)).ToArray();
        }               

        public void SelectCourse(Course course)
        {            
            if (Players.Keys.Count < Constants.NumCharacters)
            {
                throw new ApplicationException("Cannot select a course until all players have been created");
            }

            Races.Add(new Race(Players.Keys.ToArray(), course));
        }
    }
}
