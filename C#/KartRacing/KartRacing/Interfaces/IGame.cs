using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KartRacing.Interfaces
{
    public interface IGame
    {
        List<IRace> Races { get; }
        Dictionary<Character, string> Players { get; }
        RaceClass Class { get; }
        void AddPlayer(Character character, string name);
        Character[] GetUnusedCharacters();
        void SelectCourse(Course course);
    }
}
