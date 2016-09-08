using FluentAssertions;
using System;
using Xunit;

namespace KartRacing.UnitTests
{    
    public class RacePlayerTests
    {
        [Fact]
        public void LapCompleted_Default_IncrementsNumLapsByOne()
        {
            var racePlayer = new RacePlayer();
            racePlayer.LapCompleted();

            racePlayer.NumLapsCompleted.Should().Be(1);
        }

        [Fact]
        public void FinishRace_Defualt_SetsPosition()
        {
            var racePlayer = new RacePlayer();
            racePlayer.FinishRace(new TimeSpan(), 1);

            racePlayer.Position.Should().Be(1);
        }

        [Fact]
        public void FinishRace_Defualt_SetsFinishTime()
        {
            var racePlayer = new RacePlayer();
            var finishTime = new TimeSpan(0, 2, 20);

            racePlayer.FinishRace(finishTime, 1);

            racePlayer.FinishTime.Should().Be(finishTime);
        }       
    }
}
