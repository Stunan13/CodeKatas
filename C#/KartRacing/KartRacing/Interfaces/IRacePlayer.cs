using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KartRacing.Interfaces
{
    public interface IRacePlayer

    {        
        int Position { get; }
        TimeSpan FinishTime { get; }
        int NumLapsCompleted { get; }
        void LapCompleted();
        void FinishRace(TimeSpan finishTime, int position);
    }
}
