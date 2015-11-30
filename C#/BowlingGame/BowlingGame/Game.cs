using System;
using System.Collections.Generic;
using System.Linq;

namespace BowlingGame
{
    public class Game
    {        
        public List<Frame> Frames { get; set; }

        public int Score 
        { 
            get { return this.Frames.Sum(f => f.Score); } 
        }

        public Frame CurrentFrame 
        {
            get { return this.Frames.Count > 0 ? this.Frames.Last() : null; }
        }        

        public Game()
        {
            this.Frames = new List<Frame>();
        }

        public void AddFrame()
        {
            this.Frames.Add(new Frame(this.Frames.Count + 1));
        }
    }
}
