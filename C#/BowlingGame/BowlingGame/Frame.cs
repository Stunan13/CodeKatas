using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingGame
{
    public class Frame
    {
        public enum ScoreModifierType
        {
            None,
            Spare,
            Strike
        }

        public int FrameNumber { get; set; }
        public List<Turn> Turns { get; set; }
        public ScoreModifierType ScoreModifier { get; set; }
        public int Score { get; set; }

        public Frame()
        {
            this.FrameNumber = 0;
            this.Turns = new List<Turn>();
            this.Score = 0;
            this.ScoreModifier = ScoreModifierType.None;
        }

        public Frame(int frameNumber)
        {
            this.FrameNumber = frameNumber;
            this.Turns = new List<Turn>();
            this.Score = 0;
            this.ScoreModifier = ScoreModifierType.None;
        }
    }
}
