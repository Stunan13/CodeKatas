using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BowlingGame;

namespace BowlingGameTests
{
    [TestFixture]
    public class GameManagerTests
    {
        #region Helper Methods
        public GameManager MakeGameManager()
        {
            return new GameManager(new Game());
        }
        #endregion

        public void StartGame_ByDefault_AddsNewFrame() 
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();

            var expected = new List<Frame>() { new Frame() };
            var actual = gameManager.Game.Frames;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        public void Roll_AddsToScore_NumberOfPins(int numPins)
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(numPins, false);

            var expected = numPins;
            var actual = gameManager.Game.Score;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Roll_ThrowsArgumentOutOfRangeException_WhenPinsGreaterThan10()
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => gameManager.TakeTurn(11, false));

            StringAssert.Contains("Number of Pins must be between 0 and", ex.Message);
        }

        [Test]
        public void Roll_ThrowsArgumentOutOfRangeException_WhenPinsGreaterThanRemainingPinsOnSecondTurn()
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(5, false);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => gameManager.TakeTurn(6, false));

            StringAssert.Contains("Number of Pins must be between 0 and", ex.Message);
        }

        [Test]
        public void Roll_ByDefault_AddsNewTurnToFrame()
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(0, false);            

            var expected = 1;
            var actual = gameManager.Game.Frames.First().Turns.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Roll_AddsFoulTurn_WhenFoulRolled()
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(0, true);

            var expected = true;
            var actual = gameManager.Game.Frames.First().Turns.First().IsFoul;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Roll_ScoresZero_WhenFoulRolled()
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(5, true);

            var expected = 0;
            var actual = gameManager.Game.Frames.First().Score;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Roll_Twice_AddsSecondNewTurnToFrame()
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(0, false);
            gameManager.TakeTurn(0, false);

            var expected = 2;
            var actual = gameManager.Game.Frames.First().Turns.Count;

            Assert.AreEqual(expected, actual);   
        }

        [Test]
        public void Roll_AddsNewFrame_AfterTwoTurns() 
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(0, false);
            gameManager.TakeTurn(0, false);

            var expected = 2;
            var actual = gameManager.Game.Frames.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Roll_Strike_WhenTenPinsAreHitOnTheFirstTurnOfAFrame() 
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(10, false);

            var expected = Frame.ScoreModifierType.Strike;
            var actual = gameManager.Game.Frames.First().ScoreModifier;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(0, 10)]
        [TestCase(1, 9)]
        [TestCase(2, 8)]
        [TestCase(3, 7)]
        [TestCase(4, 6)]
        [TestCase(5, 5)]
        [TestCase(6, 4)]
        [TestCase(7, 3)]
        [TestCase(8, 2)]
        [TestCase(1, 9)]
        public void Roll_Spare_WhenTenPinsAreHitOverBothTurnsInFrame(int first, int second) 
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(first, false);
            gameManager.TakeTurn(second, false);

            var expected = Frame.ScoreModifierType.Spare;
            var actual = gameManager.Game.Frames.First().ScoreModifier;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Roll_AddsNewFrame_WhenStrikeIsRolled()
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(10, false);

            var expected = 2;
            var actual = gameManager.Game.Frames.Count;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(10, 1, 1, 14)]
        [TestCase(10, 10, 5, 45)]
        [TestCase(10, 10, 10, 60)]
        public void Roll_CalculateScore_IncludesNextTwoTurnsWhenStrikeIsRolled(int first, int second, int third, int expected) 
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(first, false);
            gameManager.TakeTurn(second, false);
            gameManager.TakeTurn(third, false);

            var actual = gameManager.Game.Score;

            Assert.AreEqual(expected, gameManager.Game.Score);
        }

        [TestCase(9, 1, 1, 12)]
        [TestCase(5, 5, 10, 30)]
        [TestCase(7, 3, 5, 20)]
        public void Roll_CalculateScore_IncludesNextTurnWhenSpareIsRolled(int first, int second, int third, int expected)
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();
            gameManager.TakeTurn(first, false);
            gameManager.TakeTurn(second, false);
            gameManager.TakeTurn(third, false);

            var actual = gameManager.Game.Score;

            Assert.AreEqual(expected, gameManager.Game.Score);
        }

        [Test]
        public void Roll_FinalFrameHasThreeTurns_WhenStrikeHasBeenRolled() 
        {
            var gameManager = MakeGameManager();            
            gameManager.StartGame();

            for (int i = 0; i < 12; i++) 
            {
                gameManager.TakeTurn(10, false);
            }

            int expected = 3;
            int actual = gameManager.Game.CurrentFrame.Turns.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Roll_FinalFrameHasThreeTurns_WhenSpareHasBeenRolled()
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();

            for (int i = 0; i < 21; i++)
            {
                gameManager.TakeTurn(5, false);
            }

            int expected = 3;
            int actual = gameManager.Game.CurrentFrame.Turns.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Roll_PerfectGame_ScoreEqualsThreeHundrerd()
        {
            var gameManager = MakeGameManager();
            gameManager.StartGame();

            for (int i = 0; i < 12; i++)
            {
                gameManager.TakeTurn(10, false);
            }

            int expected = 300;
            int actual = gameManager.Game.Score;

            Assert.AreEqual(expected, actual);
        }
    }
}
