using FluentAssertions;
using KartRacing.Interfaces;
using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace KartRacing.UnitTests
{
    public class LeaderboardTests
    {
        [Theory]
        [InlineData(1, 9)]
        [InlineData(2, 6)]
        [InlineData(3, 3)]
        [InlineData(4, 1)]
        [InlineData(5, 0)]
        [InlineData(6, 0)]
        [InlineData(7, 0)]
        [InlineData(8, 0)]
        public void CalculatePoints_Default_SetsPointsBasedOnPosition(int position, int expectedPoints)
        {
            var leaderboard = new Leaderboard();
            leaderboard.CalculatePoints(position).Should().Be(expectedPoints);
        }

        [Fact]
        public void GetPlayerScores_Default_ReturnsScoresForAllGameRaces()
        {
            var game = Substitute.For<IGame>();

            var race1 = Substitute.For<IRace>();
            var race2 = Substitute.For<IRace>();

            var player1 = Substitute.For<IRacePlayer>();
            var player2 = Substitute.For<IRacePlayer>();

            player1.Position.Returns(1);
            player2.Position.Returns(2);

            var players = new Dictionary<Character, IRacePlayer>();
            players.Add(Character.Mario, player1);
            players.Add(Character.Luigi, player2);

            race1.Players.Returns(players);
            race2.Players.Returns(players);

            var races = new List<IRace>() { race1, race2 };
            game.Races.Returns(races);

            var leaderboard = new Leaderboard();
            var expectedScores = new Dictionary<Character, int>();

            expectedScores.Add(Character.Mario, 18);
            expectedScores.Add(Character.Luigi, 12);

            leaderboard.GetPlayerScores(game).ShouldBeEquivalentTo(expectedScores);            
        }

        [Fact]
        public void GetStandings_Default_ReturnsScoresInOrder()
        {
            var leaderBoard = new Leaderboard();
            var scores = new Dictionary<Character, int>();
            scores.Add(Character.Mario, 0);
            scores.Add(Character.Luigi, 10);
            scores.Add(Character.Bowser, 5);

            var expectedStandings = new SortedDictionary<int, IPlayerScore>();
            expectedStandings.Add(1, new PlayerScore(Character.Luigi, 10));
            expectedStandings.Add(2, new PlayerScore(Character.Bowser, 5));
            expectedStandings.Add(3, new PlayerScore(Character.Mario, 0));

            leaderBoard.CalculateStandings(scores);

            leaderBoard.Standings.ShouldBeEquivalentTo(expectedStandings);
        }
    }
}
