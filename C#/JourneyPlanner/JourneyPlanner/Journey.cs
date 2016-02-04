using System.Collections.Generic;
using System.Linq;
using JourneyPlanner.Interfaces;

namespace JourneyPlanner
{
    public class Journey : IJourney
    {
        public List<IRoute> Routes { get; set; }

        public int Duration
        {
            get { return Routes.Sum(r => r.Duration); }
        }

        public Journey()
        {
            Routes = new List<IRoute>();
        }
    }
}
