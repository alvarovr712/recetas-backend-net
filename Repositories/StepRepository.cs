using RecetasAPINet.Data;
using RecetasAPINet.Models;

public class StepRepository : IStepRepository
{
    private readonly RecetasDbContext _context;

    public StepRepository(RecetasDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(List <Step> steps)
    {
        await _context.Steps.AddRangeAsync(steps);
    }
}