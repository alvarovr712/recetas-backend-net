namespace RecetasAPINet.Repositories
{
    public interface ISessionRepository
    {
        Task<int> CountAllAsync();
        Task<int> CountCreatedBetweenAsync(DateTime start,DateTime end);
    }
}