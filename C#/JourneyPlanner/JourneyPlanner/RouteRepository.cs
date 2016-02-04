using System;
using System.Collections.Generic;
using System.Linq;
using JourneyPlanner.Interfaces;

namespace JourneyPlanner
{
    public class RouteRepository : IRouteRepository
    {
        private int _nextId;
        private readonly List<IRoute> _routes;
        private readonly IRouteFactory _routeFactory;

        public RouteRepository(IRouteFactory routeFactory)
        {
            _routeFactory = routeFactory;
            _routes = new List<IRoute>();
            _nextId = 1;
        }

        public IEnumerable<IRoute> GetRoutes()
        {
            return _routes;
        }

        public IRoute GetRoute(string from, string to)
        {
            var route = _routes.FirstOrDefault(r => r.From == from && r.To == to);
            if (route == null)
            {
                string errorMessage = string.Format(@"Invalid Route: No route available from {0} to {1}", from, to);
                throw new ArgumentException(errorMessage);
            }

            return route;
        }

        public void AddRoute(string from, string to, int duration)
        {
            var routeToAdd = _routeFactory.MakeRoute(_nextId, from, to, duration);
            if (!RouteExists(routeToAdd))
            {
                _routes.Add(routeToAdd);
                _nextId++;
            }
            else
            {
                throw new ArgumentException(string.Format(@"Route {0} -> {1} already exists", from, to));
            }
        }

        public void DeleteRoute(IRoute route)
        {
            if (RouteExists(route))
            {
                var routeToDelete = _routes.FirstOrDefault(r => r.Equals(route));
                _routes.Remove(routeToDelete);
            }
            else
            {
                throw new ArgumentException(string.Format(@"Route {0} -> {1} does not exist and cannot be deleted", route.From, route.To));
            }
        }

        public void UpdateRoute(IRoute route)
        {
            if (RouteExists(route))
            {
                int routeIndex = _routes.FindIndex(r => r.Id == route.Id);
                _routes[routeIndex] = route;
            }
            else
            {
                throw new ArgumentException(string.Format(@"Route {0} -> {1} does not exist and cannot be updated", route.From, route.To));
            }
        }

        private bool RouteExists(IRoute route)
        {
            return _routes.Any(r => r.From == route.From && r.To == route.To);
        }
    }
}
