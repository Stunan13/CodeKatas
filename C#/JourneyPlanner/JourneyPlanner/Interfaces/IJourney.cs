using System.Collections.Generic;

namespace JourneyPlanner.Interfaces
{
    public interface IJourney
    {
        List<IRoute> Routes { get; set; }
        int Duration { get; }
    }
}
