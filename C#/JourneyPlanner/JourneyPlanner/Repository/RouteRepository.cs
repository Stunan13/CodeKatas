using System;
using System.Collections.Generic;
using System.Linq;

namespace JourneyPlanner
{
    public class RouteRepository : IRouteRepository
    {
        private int _nextId;
        private readonly List<Route> _routes;

        public RouteRepository()
        {
            _routes = new List<Route>();
            _nextId = 1;
        }

        public IEnumerable<Route> GetRoutes()
        {
            return _routes;
        }

        public Route GetRoute(string fromPort, string toPort)
        {
            var route = _routes.FirstOrDefault(r => r.PortFrom == fromPort && r.PortTo == toPort);
            if (route == null)
            {
                string errorMessage = string.Format(@"Invalid Route: No route available from {0} to {1}", fromPort, toPort);
                throw new ArgumentException(errorMessage);
            }

            return route;
        }

        public void AddRoute(string portFrom, string portTo, int duration)
        {
            var routeToAdd = new Route
            {
                PortFrom = portFrom,
                PortTo = portTo,
                Duration = duration,
                Id = _nextId
            };

            if (!RouteExists(routeToAdd))
            {
                _routes.Add(routeToAdd);
                _nextId++;
            }
            else
            {
                throw new ArgumentException(string.Format(@"Route {0} -> {1} already exists", portFrom, portTo));
            }
        }

        public void DeleteRoute(Route route)
        {
            if (RouteExists(route))
            {
                var routeToDelete = _routes.FirstOrDefault(r => r == route);
                _routes.Remove(routeToDelete);
            }
            else
            {
                throw new ArgumentException(string.Format(@"Route {0} -> {1} does not exist and cannot be deleted", route.PortFrom, route.PortTo));
            }
        }

        public void UpdateRoute(Route route)
        {
            if (RouteExists(route))
            {
                int routeIndex = _routes.FindIndex(r => r.Id == route.Id);
                _routes[routeIndex] = route;
            }
            else
            {
                throw new ArgumentException(string.Format(@"Route {0} -> {1} does not exist and cannot be updated", route.PortFrom, route.PortTo));
            }
        }

        private bool RouteExists(Route route)
        {
            return _routes.Any(r => r.PortFrom == route.PortFrom && r.PortTo == route.PortTo);
        }
    }
}
