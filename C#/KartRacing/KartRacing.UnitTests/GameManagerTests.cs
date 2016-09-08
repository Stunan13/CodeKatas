using FluentAssertions;
using KartRacing.Interfaces;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace KartRacing.UnitTests
{
    public class GameManagerTests
    {
        [Fact]
        public void SelectCourse_MaxCoursesSelected_ThrowsArgumentOutOfRangeException()
        {
            var game = Substitute.For<IGame>();
            var race = Substitute.For<IRace>();
            var manager = new GameManager();
            manager.Game = game;
            manager.Type = GameType.SingleRace;                
            game.Races.Returns(new List<IRace>() { race });

            Action action = () => manager.SelectCourse(Course.RainbowRoad);

            action.ShouldThrow<ArgumentOutOfRangeException>().And
                  .Message.Should().Contain(string.Format("The maximum of: {0} course(s) have already been selected", Constants.SingleRaceNumRaces));

        }

        [Fact]
        public void SelectCourse_Default_CallsSelectCourse()
        {
            var game = Substitute.For<IGame>();
            var manager = new GameManager();
            manager.Type = GameType.SingleRace;            
            manager.Game = game;
            game.Races.Returns(new List<IRace>());

            manager.SelectCourse(Course.BowsersCastle);

            game.Received().SelectCourse(Course.BowsersCastle);
        }

        [Fact]
        public void CreateCpuPlayers_Default_CreatesPlayersFromUnusedCharacters()
        {
            var characters = new Character[] { Character.Mario, Character.Luigi };
            var game = Substitute.For<IGame>();
            var manager = new GameManager();            
            manager.Game = game;

            game.GetUnusedCharacters().Returns(characters);
            manager.CreateCpuPlayers();

            game.Received(characters.Length).AddPlayer(Arg.Any<Character>(), Arg.Any<string>());
        }       
    }
}
