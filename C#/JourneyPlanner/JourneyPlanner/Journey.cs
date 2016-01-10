using System.Collections.Generic;
using System.Linq;

namespace JourneyPlanner
{
    public class Journey
    {
        public List<Route> Routes { get; set; }

        public int Duration
        {
            get { return Routes.Sum(r => r.Duration); }
        }

        public Journey()
        {
            Routes = new List<Route>();
        }
    }
}
