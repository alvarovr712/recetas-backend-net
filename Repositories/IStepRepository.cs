using RecetasAPINet.Models;

public interface IStepRepository
{
    Task AddRangeAsync(List<Step> steps);
}