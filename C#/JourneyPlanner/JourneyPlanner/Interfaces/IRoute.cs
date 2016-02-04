namespace JourneyPlanner.Interfaces
{
    public interface IRoute
    {
        int Id { get; set; }
        string From { get; set; }
        string To { get; set; }
        int Duration { get; set; }
    }
}
