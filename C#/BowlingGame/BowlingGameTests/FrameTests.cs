using BowlingGame;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BowlingGameTests
{
    [TestFixture]
    class FrameTests
    {
        [Test]
        public void Frame_ByDefault_ScoreModifierIsSetToNone()
        {
            var frame = new Frame();

            var expected = Frame.ScoreModifierType.None;
            var actual = frame.ScoreModifier;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Frame_ByDefault_HasEmptyListOfTurns()
        {
            var frame = new Frame();

            var expected = new List<Turn>();
            var actual = frame.Turns;

            Assert.AreEqual(expected, actual);
        }
    }
}
