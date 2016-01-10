using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using JourneyPlanner;

namespace JourneyPlanner.Tests
{
    [TestFixture]
    public class JourneyPlannerTests
    {
        #region Test Data

        private List<Route> fakeRoutesDb = new List<Route>
        {         
            new Route { Id = 1, PortFrom = "Buenos Aires", PortTo = "New York", Duration = 6 },
            new Route { Id = 2, PortFrom = "Buenos Aires", PortTo = "Casablanca", Duration = 5 },
            new Route { Id = 3, PortFrom = "Buenos Aires", PortTo = "Cape Town", Duration = 4 },
            new Route { Id = 4, PortFrom = "Cape Town", PortTo = "New York", Duration = 4 },                
            new Route { Id = 5, PortFrom = "Casablanca", PortTo = "Liverpool", Duration = 3 },
            new Route { Id = 6, PortFrom = "Casablanca", PortTo = "Cape Town", Duration = 6 },                
            new Route { Id = 7, PortFrom = "Liverpool", PortTo = "Casablanca", Duration = 3 },
            new Route { Id = 8, PortFrom = "Liverpool", PortTo = "Cape Town", Duration = 6},
            new Route { Id = 9, PortFrom = "New York", PortTo = "Liverpool", Duration = 8 }
        };

        #endregion

        #region Helper Methods

        private IRouteRepository MakeFakeRouteRepository()
        {
            var fakeRepository = Substitute.For<IRouteRepository>();

            return Substitute.For<IRouteRepository>();
        }

        private JourneyPlanner MakeJourneyPlanner(IRouteRepository routeRepository)
        {
            return new JourneyPlanner(routeRepository);
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
                          .Returns(getRoute => fakeRoutesDb.Single(r => r.PortFrom == getRoute[0].ToString() && r.PortTo == getRoute[1].ToString()));

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
                          .Returns(getRoute => fakeRoutesDb.Single(r => r.PortFrom == getRoute[0].ToString() && r.PortTo == getRoute[1].ToString()));

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

            mockRepository.GetRoutes().Returns(fakeRoutesDb);

            var actual = journeyPlanner.CreatePossibleJourneysBetweenPorts(portFrom, portTo).Count();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Buenos Aires", "Liverpool", 8)]
        [TestCase("New York", "New York", 18)]
        public void FindShortestJourneysBetweenPorts_ByDefault_ReturnsExpectedShortestJourney(string portFrom, string portTo, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(fakeRoutesDb);

            var actual = journeyPlanner.FindShortestJourneysBetweenPorts(portFrom, portTo).Duration;
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Buenos Aires", "Liverpool", 3, 2)]
        [TestCase("Buenos Aires", "Cape Town", 3, 3)]
        public void FindJourneysByFilter_WithMinStopsFilter_ReturnsExpectedNumberOfJourneys(string portFrom, string portTo, int minStops, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(fakeRoutesDb);

            var actual = journeyPlanner.FindJourneysByFilter(portFrom, portTo, JourneyPlanner.JourneyFilter.MinStops, minStops).Count();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Liverpool", "Liverpool", 3, 2)]
        [TestCase("New York", "Cape Town", 3, 2)]
        public void FindJourneysByFilter_WithMaxStopsFilter_ReturnsExpectedNumberOfJourneys(string portFrom, string portTo, int maxStops, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(fakeRoutesDb);

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

            mockRepository.GetRoutes().Returns(fakeRoutesDb);

            var actual = journeyPlanner.FindJourneysByFilter(portFrom, portTo, JourneyPlanner.JourneyFilter.ExactStops, numStops).Count();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Liverpool", "Liverpool", 25, 3)]
        [TestCase("Liverpool", "Liverpool", 6, 1)]
        public void FindJourneysByFilter_WithMaxDurationFilter_ReturnsExpectedNumberOfJourneys(string portFrom, string portTo, int maxDuration, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(fakeRoutesDb);

            var actual = journeyPlanner.FindJourneysByFilter(portFrom, portTo, JourneyPlanner.JourneyFilter.MaxDuration, maxDuration).Count();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("Liverpool", "Liverpool", 25, 0)]
        [TestCase("Liverpool", "Liverpool", 6, 3)]
        public void FindJourneysByFilter_WithMinDurationFilter_ReturnsExpectedNumberOfJourneys(string portFrom, string portTo, int minDuration, int expected)
        {
            var mockRepository = MakeFakeRouteRepository();
            var journeyPlanner = MakeJourneyPlanner(mockRepository);

            mockRepository.GetRoutes().Returns(fakeRoutesDb);

            var actual = journeyPlanner.FindJourneysByFilter(portFrom, portTo, JourneyPlanner.JourneyFilter.MinDuration, minDuration).Count();
            Assert.AreEqual(expected, actual);
        }
    }
}
