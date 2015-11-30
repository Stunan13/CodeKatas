using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingGame
{
    public class GameManager
    {
        public const int NumPinsPerTurn = 10;
        public const int NumFramesPerGame = 10;
        public const int MaxTurnsPerFrame = 2;
        public const int MaxTurnsOnFinalFrame = 3;        

        #region Public Properties

        public Game Game { get; set; }

        #endregion

        #region Constructor
        public GameManager(Game game)
        {
            this.Game = game;
        }
        #endregion

        #region Public Methods

        public void StartGame()
        {
            this.Game.AddFrame();
        }

        public void TakeTurn(int pins, bool isFoul)
        {
            if (!IsFrameCompleted())
            {
                if (!isFoul)
                {
                    Roll(pins);
                }
                else
                {
                    FoulTurn();
                }

                UpdateScores();
                FinishTurn();
            }
        }

        #endregion

        #region Private Methods

        private void Roll(int pins)
        {
            int pinsRemaining = !IsFinalFrame() ? NumPinsPerTurn - this.Game.CurrentFrame.Score : NumPinsPerTurn;
            if (pins >= 0 && pins <= pinsRemaining)
            {
                this.Game.CurrentFrame.Turns.Add(new Turn { Pins = pins });

                if (IsStrike())
                {
                    StrikeRolled();
                }
                else if (IsSpare())
                {
                    SpareRolled();
                }   
            }
            else
            {
                throw new ArgumentOutOfRangeException(string.Format("Number of Pins must be between 0 and {0}", pinsRemaining));
            }
        }

        private void FoulTurn()
        {
            this.Game.CurrentFrame.Turns.Add(new Turn { Pins = 0, IsFoul = true });
        }        

        private void StrikeRolled()
        {
            this.Game.CurrentFrame.ScoreModifier = Frame.ScoreModifierType.Strike;
        }

        private void SpareRolled()
        {
            this.Game.CurrentFrame.ScoreModifier = Frame.ScoreModifierType.Spare;
        }

        private bool IsFinalFrame()
        {
            return this.Game.Frames.Count == NumFramesPerGame;
        }

        private bool CanTakeExtraTurnOnFinalFrame() 
        {
            return this.Game.CurrentFrame.Score >= 10;
        }

        private bool IsFrameCompleted()
        {
            return this.Game.CurrentFrame.Turns.Count == (IsFinalFrame() && CanTakeExtraTurnOnFinalFrame() ? MaxTurnsOnFinalFrame : MaxTurnsPerFrame) || (!IsFinalFrame() && IsStrike());
        }

        private bool IsStrike()
        {
            var frame = this.Game.CurrentFrame;
            return !IsFinalFrame() && frame.Turns.Count == 1 && frame.Turns.First().Pins == NumPinsPerTurn;
        }

        private bool IsSpare()
        {
            var frame = this.Game.CurrentFrame;
            return frame.Turns.Count > 1 && frame.Turns.Sum(t => t.Pins) == NumPinsPerTurn;
        }

        private void FinishTurn()
        {
           if (IsFrameCompleted() && !IsFinalFrame())
            {
                this.Game.AddFrame();
            }
        }

        private void UpdateScores()
        {
            List<Frame> frames = this.Game.Frames;

            for (int i = 0; i < frames.Count; i++)
            {
                if (frames[i].ScoreModifier == Frame.ScoreModifierType.Spare)
                {
                    frames[i].Score = CalculateSpareScore(frames, i);
                }
                else if (frames[i].ScoreModifier == Frame.ScoreModifierType.Strike)
                {
                    frames[i].Score = CalculateStrikeScore(frames, i);
                }
                else
                {
                    frames[i].Score = frames[i].Turns.Sum(t => t.Pins);
                }
            }
        }

        private int CalculateSpareScore(List<Frame> frames, int frameIndex)
        {
            int scoretoAdd = NumPinsPerTurn;

            if (frames.Count > frameIndex + 1)
            {
                scoretoAdd += frames[frameIndex + 1].Turns.First().Pins;
            }

            return scoretoAdd;
        }

        private int CalculateStrikeScore(List<Frame> frames, int frameIndex)
        {
            int scoreToAdd = NumPinsPerTurn;
            Turn[] turns = frames.Where(f => f.FrameNumber > (frameIndex +1 ))
                                 .SelectMany(f => f.Turns)
                                 .ToArray();

            int turnsToAdd = turns.Length < 2 ? turns.Length : 2;           

            for (int i = 0; i < turnsToAdd; i++)
            {
                scoreToAdd += turns[i].Pins;
            }

            return scoreToAdd;
        }

        #endregion
    }
}
