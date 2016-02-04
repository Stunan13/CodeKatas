using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JourneyPlanner;
using JourneyPlanner.Interfaces;
using NUnit.Framework;
using NSubstitute;

namespace JourneyPlanner.Tests
{
    [TestFixture]
    public class RouteRepositoryTests
    {
        #region Helper Methods

        private RouteRepository MakeRouteRepository(IRouteFactory routeFactory)
        {
            return new RouteRepository(routeFactory);
        }

        private IRouteFactory MakeFakeRouteFactory()
        {
            return Substitute.For<IRouteFactory>();
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
            var routeFactory = MakeFakeRouteFactory();
            var routeRepository = MakeRouteRepository(routeFactory);

            var expected = 0;
            var actual = routeRepository.GetRoutes().Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetRoutes_ReturnsListWithOneEntry_WhenRouteIsAdded()
        {
            var routeFactory = MakeFakeRouteFactory();
            var routeRepository = MakeRouteRepository(routeFactory);
            var ports = MakePorts();
            

            routeRepository.AddRoute(ports[0], ports[1], 4);
            var expected = 1;
            var actual = routeRepository.GetRoutes().Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetRoutes_ReturnsEmptyList_WhenRouteIsAddedThenDeleted()
        {
            var routeFactory = MakeFakeRouteFactory();
            var routeRepository = MakeRouteRepository(routeFactory);
            var ports = MakePorts();

            routeFactory.MakeRoute(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>())
                        .Returns(r => MakeFakeRoute(Convert.ToInt32(r[0]), Convert.ToString(r[1]), Convert.ToString(r[2]), Convert.ToInt32(r[3])));

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
            var routeFactory = MakeFakeRouteFactory();
            var routeRepository = MakeRouteRepository(routeFactory);
            var ports = MakePorts();

            routeFactory.MakeRoute(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>())
                        .Returns(r => MakeFakeRoute(Convert.ToInt32(r[0]), Convert.ToString(r[1]), Convert.ToString(r[2]), Convert.ToInt32(r[3])));

            routeRepository.AddRoute(ports[0], ports[1], 4);
            var route = routeRepository.GetRoute(ports[0], ports[1]);

            var expected = ports;
            var actual = new[] { route.From, route.To };

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void GetRoute_ThrowsArgumentException_WhenRouteDoesNotExist()
        {
            var routeFactory = MakeFakeRouteFactory();
            var routeRepository = MakeRouteRepository(routeFactory);
            var ports = MakePorts();

            var expected = "Invalid Route";
            var ex = Assert.Throws<ArgumentException>(() => routeRepository.GetRoute(ports[0], ports[1]));

            StringAssert.Contains(expected, ex.Message);
        }

        [Test]
        public void AddRoute_ThrowsArgumentException_WhenRouteAlreadyExistsWithSamePorts()
        {
            var routeFactory = MakeFakeRouteFactory();
            var routeRepository = MakeRouteRepository(routeFactory);
            var ports = MakePorts();

            routeFactory.MakeRoute(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>())
                        .Returns(r => MakeFakeRoute(Convert.ToInt32(r[0]), Convert.ToString(r[1]), Convert.ToString(r[2]), Convert.ToInt32(r[3])));

            routeRepository.AddRoute(ports[0], ports[1], 4);

            var expected = "already exists";
            var ex = Assert.Throws<ArgumentException>(() => routeRepository.AddRoute(ports[0], ports[1], 4));

            StringAssert.Contains(expected, ex.Message);
        }

        [Test]
        public void UpdateRoute_ByDefault_ChangesValuesOfRouteWithSameId()
        {
            var routeFactory = MakeFakeRouteFactory();
            var routeRepository = MakeRouteRepository(routeFactory);
            var ports = MakePorts();

            routeFactory.MakeRoute(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>())
                        .Returns(r => MakeFakeRoute(Convert.ToInt32(r[0]), Convert.ToString(r[1]), Convert.ToString(r[2]), Convert.ToInt32(r[3])));

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
            var routeFactory = MakeFakeRouteFactory();
            var routeRepository = MakeRouteRepository(routeFactory);

            var ports = MakePorts();
            var route =  MakeFakeRoute(1, ports[0], ports[1], 1);

            var expected = "does not exist";
            var ex = Assert.Throws<ArgumentException>(() => routeRepository.UpdateRoute(route));

            StringAssert.Contains(expected, ex.Message);
        }
    }
}
