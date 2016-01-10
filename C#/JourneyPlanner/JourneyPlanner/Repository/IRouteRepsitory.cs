using System.Collections.Generic;

namespace JourneyPlanner
{
    public interface IRouteRepository
    {
        IEnumerable<Route> GetRoutes();
        Route GetRoute(string fromPort, string toPort);
        void AddRoute(string portFrom, string portTo, int duration);
        void DeleteRoute(Route route);
        void UpdateRoute(Route route);
    }
}
