using System.Collections.Generic;
using JourneyPlanner.Interfaces;

namespace JourneyPlanner
{
    public interface IRouteRepository
    {
        IEnumerable<IRoute> GetRoutes();
        IRoute GetRoute(string from, string to);
        void AddRoute(string from, string to, int duration);
        void DeleteRoute(IRoute route);
        void UpdateRoute(IRoute route);
    }
}
