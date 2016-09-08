using KartRacing.Interfaces;
using System;

namespace KartRacing
{
    public class RacePlayer : IRacePlayer
    {       
        
        public int Position { get; set; }        
        public TimeSpan FinishTime { get; set; }
        public int NumLapsCompleted { get; set; }             

        public void LapCompleted()
        {
            NumLapsCompleted++;
        }

        public void FinishRace(TimeSpan finishTime, int position)
        {           
            FinishTime = finishTime;
            Position = position;            
        }
    }
}
