using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingGame
{
    public class Turn
    {
        public int Pins { get; set; }
        public bool IsFoul { get; set; }

        public Turn()
        {
            this.Pins = 0;
            this.IsFoul = false;
        }
    }
}
