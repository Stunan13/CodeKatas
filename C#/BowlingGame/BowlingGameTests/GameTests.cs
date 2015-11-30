using System;
using System.Collections.Generic;
using System.Linq;
using BowlingGame;
using NUnit.Framework;

namespace BowlingGameTests
{
    [TestFixture]
    class GameTests
    {
        #region Helper Methods
        private static Game MakeGame()
        {
            return new Game();
        }
        #endregion

        [Test]
        public void Game_ByDefault_HasEmptyListOfFrames()
        {
            var game = MakeGame();
            
            var expected = new List<Frame>();
            var actual = game.Frames;

            Assert.AreEqual(expected, actual);
        }

       
        [Test]
        public void CurrentFrame_ByDefault_ReturnsNullFrameObject()
        {
            var game = MakeGame();

            Frame expected = null;
            Frame actual = game.CurrentFrame;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void CurrentFrame_Get_ReturnsCorrectFrameInstance(int numFrames)
        {
            var game = MakeGame();

            for (var i = 0; i < numFrames; i++)
            {
                game.Frames.Add(new Frame { Score = numFrames });
            }

            var expected = game.Frames[numFrames - 1];
            var actual = game.CurrentFrame;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(0, 0)]
        [TestCase(1, 5)]
        [TestCase(2, 10)]
        [TestCase(3, 15)]
        public void Score_ByDefault_ReturnsCorrectSumOfFrameTotals(int numFrames, int score)
        {
            var game = MakeGame();

            for (var i = 0; i < numFrames; i++)
            {
                game.Frames.Add(new Frame { Score = 5 });
            }

            var expected = score;
            var actual = game.Score;

            Assert.AreEqual(expected, actual);
        }        
    }
}
