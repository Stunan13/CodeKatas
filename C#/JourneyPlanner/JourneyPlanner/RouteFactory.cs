using JourneyPlanner.Interfaces;

namespace JourneyPlanner
{
    public class RouteFactory : IRouteFactory
    {
        public IRoute MakeRoute(int id, string from, string to, int duration)
        {
            return new Route(id, from, to, duration);
        }
    }
}
