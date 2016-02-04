using System;
using JourneyPlanner.Interfaces;

namespace JourneyPlanner
{
    public class Route : IRoute
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int Duration { get; set; }
        
        public Route(int id, string from, string to, int duration)
        {
            this.Id = id;
            this.From = from;
            this.To = to;
            this.Duration = duration;
        }
    }
}
