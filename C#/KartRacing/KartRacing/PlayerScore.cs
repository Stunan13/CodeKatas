using KartRacing.Interfaces;

namespace KartRacing
{
    public class PlayerScore : IPlayerScore
    {
        public Character Character { get; set; }
        public int Points { get; set; }

        public PlayerScore(Character character, int points)
        {
            this.Character = character;
            this.Points = points;
        }
    }
}
