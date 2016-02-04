namespace JourneyPlanner.Interfaces
{
    public interface IRouteFactory
    {
        IRoute MakeRoute(int id, string from, string to, int duration);
    }
}
