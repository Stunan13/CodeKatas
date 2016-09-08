using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KartRacing.Interfaces
{
    public interface IRace
    {
        Course Course { get; }
        Dictionary<Character, IRacePlayer> Players { get; }
        DateTime StartDate { get; }        
        void LapCompleted(Character character, DateTime finishDate);
        void FinishRace(Character character, DateTime finishDate);
        void StartRace(DateTime startTime);
        void CreatePlayers(Character[] players);
    }
}
