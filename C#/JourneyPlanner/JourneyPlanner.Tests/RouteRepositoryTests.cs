using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JourneyPlanner;
using NUnit.Framework;

namespace JourneyPlanner.Tests
{
    [TestFixture]
    public class RouteRepositoryTests
    {
        #region Helper Methods

        private RouteRepository MakeRouteRepository()
        {
            return new RouteRepository();
        }

        private string[] MakePorts()
        {
            return new[]
            {
                "Liverpool",
                "New York" 
            };
        }

        #endregion

        [Test]
        public void GetRoutes_ByDefault_ReturnsEmptyList()
        {
            var routeRepository = MakeRouteRepository();

            var expected = 0;
            var actual = routeRepository.GetRoutes().Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetRoutes_ReturnsListWithOneEntry_WhenRouteIsAdded()
        {
            var routeRepository = MakeRouteRepository();
            var ports = MakePorts();

            routeRepository.AddRoute(ports[0], ports[1], 4);
            var expected = 1;
            var actual = routeRepository.GetRoutes().Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetRoutes_ReturnsEmptyList_WhenRouteIsAddedThenDeleted()
        {
            var routeRepository = MakeRouteRepository();

            var ports = MakePorts();
            routeRepository.AddRoute(ports[0], ports[1], 4);

            var routeToDelete = routeRepository.GetRoute(ports[0], ports[1]);
            routeRepository.DeleteRoute(routeToDelete);

            var expected = 0;
            var actual = routeRepository.GetRoutes().Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetRoute_ByDefault_ReturnsRouteWithMatchingFromAndToPort()
        {
            var routeRepository = MakeRouteRepository();

            var ports = MakePorts();
            routeRepository.AddRoute(ports[0], ports[1], 4);
            var route = routeRepository.GetRoute(ports[0], ports[1]);

            var expected = ports;
            var actual = new[] { route.PortFrom, route.PortTo };

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void GetRoute_ThrowsArgumentException_WhenRouteDoesNotExist()
        {
            var routeRepository = MakeRouteRepository();
            var ports = MakePorts();

            var expected = "Invalid Route";
            var ex = Assert.Throws<ArgumentException>(() => routeRepository.GetRoute(ports[0], ports[1]));

            StringAssert.Contains(expected, ex.Message);
        }

        [Test]
        public void AddRoute_ThrowsArgumentException_WhenRouteAlreadyExistsWithSamePorts()
        {
            var routeRepository = MakeRouteRepository();
            var ports = MakePorts();

            routeRepository.AddRoute(ports[0], ports[1], 4);

            var expected = "already exists";
            var ex = Assert.Throws<ArgumentException>(() => routeRepository.AddRoute(ports[0], ports[1], 4));

            StringAssert.Contains(expected, ex.Message);
        }

        [Test]
        public void UpdateRoute_ByDefault_ChangesValuesOfRouteWithSameId()
        {
            var routeRepository = MakeRouteRepository();
            var ports = MakePorts();

            routeRepository.AddRoute(ports[0], ports[1], 4);
            var route = routeRepository.GetRoute(ports[0], ports[1]);
            route.Duration = 3;

            routeRepository.UpdateRoute(route);

            var expected = 3;
            var actual = route.Duration;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateRoute_ThrowsArgumentException_WhenRouteDoesNotExistWithSameId()
        {
            var routeRepository = MakeRouteRepository();
            var ports = MakePorts();
            var route = new Route { Id = 1, PortFrom = ports[0], PortTo = ports[1] };

            var expected = "does not exist";
            var ex = Assert.Throws<ArgumentException>(() => routeRepository.UpdateRoute(route));

            StringAssert.Contains(expected, ex.Message);
        }
    }
}
