using System;
using System.Collections.Generic;
using System.Linq;
using JourneyPlanner.Interfaces;

namespace JourneyPlanner
{
    public class JourneyPlanner
    {
        public enum JourneyFilter
        {
            MinStops,
            MaxStops,
            ExactStops,
            MinDuration,
            MaxDuration
        }

        readonly IRouteRepository _routeRepository;

        public JourneyPlanner(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        public Journey CreateJourneyForExactPorts(string[] ports)
        {
            var journey = new Journey();

            try
            {
                for (var i = 0; i < ports.Length - 1; i++)
                {
                    journey.Routes.Add(_routeRepository.GetRoute(ports[i], ports[i + 1]));
                }
            }
            catch (ArgumentException ae)
            {
                throw ae;
            }

            return journey;
        }

        public Journey FindShortestJourneysBetweenPorts(string portFrom, string portTo)
        {
            return CreatePossibleJourneysBetweenPorts(portFrom, portTo).OrderBy(j => j.Duration).FirstOrDefault();
        }

        public Journey[] FindJourneysByFilter(string portFrom, string portTo, JourneyFilter filterType, int value)
        {
            var journeys = CreatePossibleJourneysBetweenPorts(portFrom, portTo);

            switch (filterType)
            {
                case JourneyFilter.MinStops:
                    journeys = journeys.Where(j => j.Routes.Count >= value).ToArray();
                    break;
                case JourneyFilter.MaxStops:
                    journeys = journeys.Where(j => j.Routes.Count <= value).ToArray();
                    break;
                case JourneyFilter.ExactStops:
                    journeys = journeys.Where(j => j.Routes.Count == value).ToArray();
                    break;
                case JourneyFilter.MinDuration:
                    journeys = journeys.Where(j => j.Duration >= value).ToArray();
                    break;
                case JourneyFilter.MaxDuration:
                    journeys = journeys.Where(j => j.Duration <= value).ToArray();
                    break;
            }

            return journeys;
        }

        public Journey[] CreatePossibleJourneysBetweenPorts(string portFrom, string portTo)
        {
            var journeys = new List<Journey>();
            var routes = _routeRepository.GetRoutes();

            FindRoutesRecursive(portFrom, portTo, new List<string>(), ref routes, ref journeys);

            return journeys.ToArray();
        }

        private void FindRoutesRecursive(string portFrom, string portTo, List<string> portsVisited, ref IEnumerable<IRoute> routes, ref List<Journey> journeys)
        {
            foreach (IRoute r in routes.Where(r => r.From == portFrom && !portsVisited.Contains(r.From)))
            {
                var newPortsVisited = new List<string>(portsVisited);
                newPortsVisited.Add(r.From);

                if (r.To == portTo)
                {
                    newPortsVisited.Add(portTo);
                    journeys.Add(CreateJourneyFromRecursivePortsVisited(newPortsVisited.ToArray(), ref routes));
                }
                else
                {
                    FindRoutesRecursive(r.To, portTo, newPortsVisited, ref routes, ref journeys);
                }
            }
        }

        private Journey CreateJourneyFromRecursivePortsVisited(string[] ports, ref IEnumerable<IRoute> routes)
        {
            var journey = new Journey();

            for (var i = 0; i < ports.Length - 1; i++)
            {
                journey.Routes.Add(routes.Single(r => r.From == ports[i] && r.To == ports[i + 1]));
            }

            return journey;
        }
    }
}
