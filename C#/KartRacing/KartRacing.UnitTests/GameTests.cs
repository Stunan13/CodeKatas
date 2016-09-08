using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KartRacing.UnitTests
{
    public class GameTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void AddPlayer_PlayerNullOrWhiteSpace_ThrowsArgumentNullException(string playerName)
        {
            var game = new Game(RaceClass.FiftyCc);

            Action action = () => game.AddPlayer(Character.Mario, playerName);
            action.ShouldThrow<ArgumentNullException>().And.Message.Contains("Player cannot be empty");
        }

        [Fact]
        public void AddPlayer_CharacterExists_ThrowsArgumentException()
        {
            var game = new Game(RaceClass.FiftyCc);
            game.AddPlayer(Character.Mario, "Player 1");

            Action action = () => game.AddPlayer(Character.Mario, "Player 2");
            action.ShouldThrow<ArgumentException>().And.Message.Contains("Character already selected");
        }

        [Fact]
        public void AddPlayer_PlayerExists_ThrowsArgumentException()
        {
            var game = new Game(RaceClass.FiftyCc);
            game.AddPlayer(Character.Mario, "Player 1");

            Action action = () => game.AddPlayer(Character.Luigi, "Player 1");
            action.ShouldThrow<ArgumentException>().And.Message.Contains("Player already entered");
        }

        [Fact]
        public void AddPlayer_PlayerDoesntExist_AddsPlayer()
        {
            var game = new Game(RaceClass.FiftyCc);
            game.AddPlayer(Character.Mario, "player 1");

            game.Players.Should().HaveCount(1).And.ContainKey(Character.Mario);
        }

        [Fact]
        public void GetUnusedPlayers_NoPlayersEntered_GetsAllCharacters()
        {
            var game = new Game(RaceClass.FiftyCc);
            int numCharacters = Enum.GetNames(typeof(Character)).Length;

            game.GetUnusedCharacters().Should().HaveCount(numCharacters);            
        }

        [Fact]
        public void GetUnusedPlayers_PlayersExist_GetsRemainingCharacters()
        {
            var game = new Game(RaceClass.FiftyCc);
            int numCharacters = Enum.GetNames(typeof(Character)).Length;

            game.AddPlayer(Character.Mario, "Player 1");
            game.GetUnusedCharacters();

            game.GetUnusedCharacters().Should().OnlyHaveUniqueItems().And.HaveCount(numCharacters - 1);
        }

        [Fact]
        public void SelectCourse_NotAllCharactersAssigned_ThrowsApplicationException()
        {
            var game = new Game(RaceClass.FiftyCc);

            Action action = () => game.SelectCourse(Course.RainbowRoad);

            action.ShouldThrow<ApplicationException>().And.Message.Should().Contain("Cannot select a course until all players have been created");
        }

        [Fact]
        public void SelectCourse_CharactersAssigned_CreatesRace()
        {
            var game = new Game(RaceClass.FiftyCc);
            var characters = game.GetUnusedCharacters();

            for(int i = 0; i < characters.Length; i++)
            {
                game.AddPlayer(characters[i], "CPU - " + i);
            }

            game.SelectCourse(Course.RainbowRoad);

            game.Races.Should().HaveCount(1);
        }
    }
}
