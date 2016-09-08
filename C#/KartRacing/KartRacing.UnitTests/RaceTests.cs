using FluentAssertions;
using KartRacing.Interfaces;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace KartRacing.UnitTests
{    
    public class RaceTests
    {
        [Fact]
        public void StartRace_Default_SetsStartDate()
        {
            var players = new Character[0];
            var race = new Race(players, Course.RainbowRoad);
            var startDate = new DateTime(2016, 09, 09, 11, 30, 00);

            race.StartRace(startDate);

            race.StartDate.Should().Be(startDate);
        }       

        [Fact]
        public void CreatePlayers_Default_CreatesPlayersForCharacters()
        {
            var race = new Race();
            var characters = new Character[4] { Character.Mario, Character.Luigi, Character.Yoshi, Character.Peach };

            race.CreatePlayers(characters);

            race.Players.Keys.ShouldBeEquivalentTo(characters);
         }
        
        [Fact]
        public void GetRaceFinishTime_Default_ReturnsTimeDifferenceFromStartDate()
        {
            var race = new Race();
            race.StartRace(new DateTime(2016, 9, 8, 0, 0, 0, 0));

            var finishDate = new DateTime(2016, 9, 8, 0, 2, 30, 45);
            var finishTime = race.GetRaceFinishTime(finishDate);

            finishDate.Should().HaveMinute(2).And.HaveSecond(30).And.HaveHour(0);
        }

        [Fact]
        public void GetRaceFinishTime_NoStartDate_ThrowsArgumentException()
        {
            var race = new Race();           

            Action action = () => race.GetRaceFinishTime(DateTime.Now);
            action.ShouldThrow<ArgumentException>().And.Message.Contains("Race hasn't started");
        }

        [Fact]
        public void GetNextFinishPosition_PlayersIsNull_ThrowsArgumentNullException()
        {
            var race = new Race();

            Action action = () => race.GetNextFinishPosition();
            action.ShouldThrow<ArgumentException>().And.Message.Contains("No Players created");
        }

        [Fact]
        public void GetNextFinishPosition_NoPlayers_ThrowsArgumentNullException()
        {
            var race = new Race();
            race.Players = new Dictionary<Character, IRacePlayer>();

            Action action = () => race.GetNextFinishPosition();
            action.ShouldThrow<ArgumentException>().And.Message.Contains("No Players created");
        }

        [Fact]
        public void GetNextFiishPosition_PlayersExist_ReturnsExpectedFinishPosition()
        {
            var race = new Race();
            race.CreatePlayers(new Character[3] { Character.Mario, Character.Luigi, Character.Peach });

            race.GetNextFinishPosition().Should().Be(1);
        }

        [Fact]
        public void LapCompleted_PlayerDoesntExist_ThrowsKeyNotFoundException()
        {
            var race = new Race();

            race.CreatePlayers(new Character[1] { Character.Mario });

            Action action = () => race.LapCompleted(Character.Luigi, DateTime.Now);
            action.ShouldThrow<KeyNotFoundException>().And.Message.Contains("No player found for character:");
        }

        [Fact]
        public void LapCompleted_PlayerExists_CallsLapCompleted()
        {
            var race = new Race();
            var player = Substitute.For<IRacePlayer>();                   
       
            race.Players.Add(Character.Mario, player);
            race.LapCompleted(Character.Mario, DateTime.Now);

            player.Received().LapCompleted();
        }

        [Fact]
        public void FinishRace_PlayerDoesntExist_ThrowsKeyNotFoundException()
        {
            var race = new Race();

            race.CreatePlayers(new Character[1] { Character.Mario });

            Action action = () => race.FinishRace(Character.Luigi, DateTime.Now);
            action.ShouldThrow<KeyNotFoundException>().And.Message.Contains("No player found for character:");
        }

        [Fact]
        public void FinishRace_PlayerExists_CallsFinishRace()
        {
            var race = new Race();
            var player = Substitute.For<IRacePlayer>();

            race.Players.Add(Character.Mario, player);
            race.StartRace(DateTime.Now);
            race.FinishRace(Character.Mario, DateTime.Now);

            player.Received().FinishRace(Arg.Any<TimeSpan>(), Arg.Any<int>());
        }

        [Fact]
        public void LapCompleted_FinalLap_CallsFinishRace()
        {
            var race = new Race();
            var player = Substitute.For<IRacePlayer>();
            player.NumLapsCompleted.Returns(Constants.NumLaps);

            race.Players.Add(Character.Mario, player);
            race.StartRace(DateTime.Now);
            race.LapCompleted(Character.Mario, DateTime.Now);

            player.Received().FinishRace(Arg.Any<TimeSpan>(), Arg.Any<int>());
        }
    }
}
