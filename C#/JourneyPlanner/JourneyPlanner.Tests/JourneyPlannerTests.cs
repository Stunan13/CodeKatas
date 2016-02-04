using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using JourneyPlanner.Interfaces;

namespace JourneyPlanner.Tests
{
    [TestFixture]
    public class JourneyPlannerTests
    {
        #region Test Data

        private readonly List<IRoute> _fakeRoutesDb = new List<IRoute>();

        #endregion

        #region Helper Methods

        private IRouteRepository MakeFakeRouteRepository()
        {
            PopulateFakeDb();

            return Substitute.For<IRouteRepository>();
        }

        private JourneyPlanner MakeJourneyPlanner(IRouteRepository routeRepository)
        {
            return new JourneyPlanner(routeRepository);
        }

        private IRoute MakeFakeRoute(int id, string from, string to, int duration)
        {
            var fakeRoute = Substitute.For<IRoute>();

            fakeRoute.Id = id;
            fakeRoute.Duration = duration;
            fakeRoute.From = from;
            fakeRoute.To = to;

            return fakeRoute;
        }

        private void PopulateFakeDb()
        {
            if (_fakeRoutesDb.Count <= 0)
            {
                _fakeRoutesDb.Add(MakeFakeRoute(1, "Buenos Aires", "New York", 6));
                _fakeRoutesDb.Add(MakeFakeRoute(2, "Buenos Aires", "Casablanca", 5));
                _fakeRoutesDb.Add(MakeFakeRoute(3, "Buenos Aires", "Cape Town", 4));
                _fakeRoutesDb.Add(MakeFakeRoute(4, "Cape Town", "New York", 4));
                _fakeRoutesDb.Add(MakeFakeRoute(5, "Casablanca", "Liverpool", 3));
                _fakeRoutesDb.Add(MakeFakeRoute(6, "Casablanca", "Cape Town", 6));
                _fakeRoutesDb.Add(MakeFakeRoute(7, "Liverpool", "Casablanca", 3));
                _fakeRoutesDb.Add(MakeFakeRoute(8, "Liverpool", "Cape Town", 6));
                _fakeRoutesDb.Add(MakeFakeRoute(9, "New York", "Liverpool", 8));
            }
        }

        #endregion

        [TestCase(new[] { "Buenos Aires", "New York", "Liverpool" }, 14)]
        [TestCase(new[] { "Buenos Aires", "Casablanca", "Liverpool" }, 8)]
        [TestCase(new[] { "Buenos Aires", "Cape Town", "New York", "Liverpool", "Casablanca" }, 19)]
        public void CreateJourneyForExactPorts_ByDefault_ReturnsJourneyWithExpectedDuration(string[] ports, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoute(Arg.Any<string>(), Arg.Any<string>())
                          .Returns(getRoute => _fakeRoutesDb.Single(r => r.From == getRoute[0].ToString() && r.To == getRoute[1].ToString()));

            var actual = journeyPlanner.CreateJourneyForExactPorts(ports).Duration;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(new[] { "Buenos Aires", "New York", "Liverpool" }, 2)]
        [TestCase(new[] { "Buenos Aires", "Casablanca", "Liverpool" }, 2)]
        [TestCase(new[] { "Buenos Aires", "Cape Town", "New York", "Liverpool", "Casablanca" }, 4)]
        public void CreateJourneyForExactPorts_ByDefault_ReturnsJourneyWithExpectedNumberOfRoutes(string[] ports, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoute(Arg.Any<string>(), Arg.Any<string>())
                          .Returns(getRoute => _fakeRoutesDb.Single(r => r.From == getRoute[0].ToString() && r.To == getRoute[1].ToString()));

            var actual = journeyPlanner.CreateJourneyForExactPorts(ports).Routes.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateJourneyForExactPorts_JourneyIsNotValid_WhenExceptionIsThrownFromRepository()
        {
            var ports = new[] { "Buenos Aires", "Cape Town", "Casablanca" };
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoute(Arg.Any<string>(), Arg.Any<string>())
                          .Returns(x => { throw new ArgumentException("Journey is invalid"); });

            var expected = "Journey is invalid";
            var ex = Assert.Throws<ArgumentException>(() => journeyPlanner.CreateJourneyForExactPorts(ports));

            StringAssert.Contains(expected, ex.Message);
        }

        [TestCase("Buenos Aires", "Liverpool", 4)]
        [TestCase("Liverpool", "Cape Town", 2)]
        [TestCase("Liverpool", "Liverpool", 3)]
        public void CreatePossibleJourneysBetweenPorts_ByDefault_ReturnsExpectedNumberOfJourneys(string portFrom, string portTo, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(_fakeRoutesDb);

            var actual = journeyPlanner.CreatePossibleJourneysBetweenPorts(portFrom, portTo).Count();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Buenos Aires", "Liverpool", 8)]
        [TestCase("New York", "New York", 18)]
        public void FindShortestJourneysBetweenPorts_ByDefault_ReturnsExpectedShortestJourney(string portFrom, string portTo, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(_fakeRoutesDb);

            var actual = journeyPlanner.FindShortestJourneysBetweenPorts(portFrom, portTo).Duration;
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Buenos Aires", "Liverpool", 3, 2)]
        [TestCase("Buenos Aires", "Cape Town", 3, 3)]
        public void FindJourneysByFilter_WithMinStopsFilter_ReturnsExpectedNumberOfJourneys(string portFrom, string portTo, int minStops, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(_fakeRoutesDb);

            var actual = journeyPlanner.FindJourneysByFilter(portFrom, portTo, JourneyPlanner.JourneyFilter.MinStops, minStops).Count();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Liverpool", "Liverpool", 3, 2)]
        [TestCase("New York", "Cape Town", 3, 2)]
        public void FindJourneysByFilter_WithMaxStopsFilter_ReturnsExpectedNumberOfJourneys(string portFrom, string portTo, int maxStops, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(_fakeRoutesDb);

            var actual = journeyPlanner.FindJourneysByFilter(portFrom, portTo, JourneyPlanner.JourneyFilter.MaxStops, maxStops).Count();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Cape Town", "New York", 1, 1)]
        [TestCase("Liverpool", "New York", 2, 1)]
        [TestCase("Buenos Aires", "Liverpool", 4, 1)]
        public void FindJourneysByFilter_WithExactStopsFilter_ReturnsExpectedNumberOfJourneys(string portFrom, string portTo, int numStops, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(_fakeRoutesDb);

            var actual = journeyPlanner.FindJourneysByFilter(portFrom, portTo, JourneyPlanner.JourneyFilter.ExactStops, numStops).Count();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Liverpool", "Liverpool", 25, 3)]
        [TestCase("Liverpool", "Liverpool", 6, 1)]
        public void FindJourneysByFilter_WithMaxDurationFilter_ReturnsExpectedNumberOfJourneys(string portFrom, string portTo, int maxDuration, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(_fakeRoutesDb);

            var actual = journeyPlanner.FindJourneysByFilter(portFrom, portTo, JourneyPlanner.JourneyFilter.MaxDuration, maxDuration).Count();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Liverpool", "Liverpool", 25, 0)]
        [TestCase("Liverpool", "Liverpool", 6, 3)]
        public void FindJourneysByFilter_WithMinDurationFilter_ReturnsExpectedNumberOfJourneys(string portFrom, string portTo, int minDuration, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(_fakeRoutesDb);

            var actual = journeyPlanner.FindJourneysByFilter(portFrom, portTo, JourneyPlanner.JourneyFilter.MinDuration, minDuration).Count();
            Assert.AreEqual(expected, actual);
        }
    }
}
