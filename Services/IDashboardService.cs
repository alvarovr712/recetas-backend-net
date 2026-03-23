using RecetasAPINet.DTOs;

namespace RecetasAPINet.Services
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardAsync(int? month = null, int? year = null);
    }
}