using KartRacing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KartRacing
{
    public class Race : IRace
    {          
        public Course Course { get; set; }        
        public Dictionary<Character, IRacePlayer> Players { get; set; }        
        public DateTime StartDate { get; set; }

        public Race()
        {
            Players = new Dictionary<Character, IRacePlayer>();
        }

        public Race(Character[] players, Course course) : this()
        {
            Course = course;
            CreatePlayers(players);
        }        
        
        public void LapCompleted(Character character, DateTime finishDate)
        {
            if (!Players.ContainsKey(character))
            {
                throw new KeyNotFoundException(string.Format("No player found for character: {0}", Enum.GetName(typeof(Character), character)));
            }

            var player = Players[character];
            player.LapCompleted();

            if (player.NumLapsCompleted == Constants.NumLaps)
            {
                FinishRace(character, finishDate);
            }            
        }

        public void FinishRace(Character character, DateTime finishDate)
        {
            if (!Players.ContainsKey(character))
            {
                throw new KeyNotFoundException(string.Format("No player found for character: {0}", Enum.GetName(typeof(Character), character)));
            }

            int position = GetNextFinishPosition();
            var finishTime = GetRaceFinishTime(finishDate);

            Players[character].FinishRace(finishTime, position);                        
        }

        public void StartRace(DateTime startTime)
        {
            StartDate = startTime;
        }                         
        
        public TimeSpan GetRaceFinishTime(DateTime finishDate)
        {
            if (StartDate == DateTime.MinValue)
                throw new ArgumentException("Race hasn't started");

            return finishDate - StartDate;
        }

        public int GetNextFinishPosition()
        {
            if (Players == null || Players.Count == 0)
                throw new ArgumentException("No Players created");

            return Players.Count(p => p.Value.Position > 0) + 1;
        }

        public void CreatePlayers(Character[] characters)
        {           
            foreach (var character in characters)
            {
                Players.Add(character, new RacePlayer());
            }
        }
    }
}
